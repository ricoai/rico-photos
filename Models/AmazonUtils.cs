using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ricoai.Models
{
    public class AmazonUtils
    {
        /// <summary>
        /// AWS Credentials ID.
        /// </summary>
        private string _awsId { get; set; }

        /// <summary>
        /// AWS Credential Key.
        /// </summary>
        private string _awsKey { get; set; }

        /// <summary>
        /// AWS S3 Bucket.
        /// </summary>
        private string _awsS3Bucket { get; set; }

        /// <summary>
        /// Initialize the S3 Connection.
        /// </summary>
        /// <param name="awsId">AWS ID Credientials.</param>
        /// <param name="awsKey">AWS Key Credientials.</param>
        /// <param name="awsS3Bucket">AWS S3 Bucket.</param>
        public AmazonUtils(string awsId, string awsKey, string awsS3Bucket)
        {
            this._awsId = awsId;
            this._awsKey = awsKey;
            this._awsS3Bucket = awsS3Bucket;
        }

        /// <summary>
        /// Upload the file given by the stream to the S3 Bucket.  Use the UserID as the subdirectory folder in the S3 Bucket.
        /// </summary>
        /// <param name="file">File from the HTTP post.</param>
        /// <param name="subdir">Subdirectory within the bucket.</param>
        /// <param name="fileName">File name to use for the uploaded file.  It is suggest to make the file name random.</param>
        /// <param name="thumbImageName">File name for the thumbnail.</param>
        public async Task UploadImageAndThumbToS3(IFormFile file, string subdir, string fileName, string thumbImageName)
        {
            try
            {
                // Create a connection to the S3 bucket
                using (IAmazonS3 client = new AmazonS3Client(this._awsId, this._awsKey, Amazon.RegionEndpoint.USWest2))
                {
                    TransferUtility utility = new TransferUtility(client);

                    // Create a new stream instead of OpenReadStream because the stream could be closed
                    using (var imageMemoryStream = new MemoryStream())
                    {
                        using (var thumbMemoryStream = new MemoryStream())
                        {

                            // Add the file to the memory stream
                            await file.CopyToAsync(imageMemoryStream);
                            await file.CopyToAsync(thumbMemoryStream);

                            // Create a transfer request
                            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

                            // Create a folder with the user ID as the subdirectory
                            request.BucketName = this._awsS3Bucket + @"/" + subdir;

                            // Set the file name as the key
                            request.Key = fileName;
                            request.InputStream = imageMemoryStream;
                            request.ContentType = file.ContentType;
                            request.CannedACL = S3CannedACL.PublicRead;

                            // Upload the file to S3
                            await utility.UploadAsync(request);

                            // Upload the thumbnail to S3
                            await UploadThumbToS3(thumbMemoryStream, subdir, thumbImageName);
                        }
                    }

                    //imageMemoryStream.Dispose();
                    //thumbMemoryStream.Dispose();
                }
            }
            catch(Exception ex)
            {
                Console.Out.WriteLine(ex);
            }
        }

        /// <summary>
        /// Upload the Thumbnail to AWS S3.  This will generate a thumbnail for the image.
        /// It will then generate a request and upload it to the S3.
        /// </summary>
        /// <param name="utility">Utility connected to S3.</param>
        /// <param name="fileStream">Filestream of current image file.</param>
        /// <param name="subdir">Subdirectory to store the image.</param>
        /// <param name="thumbFileName">File name for the thumbnail.</param>
        /// <returns></returns>
        private async Task UploadThumbToS3(Stream memoryStream, string subdir, string thumbFileName)
        {
            // Create a connection to the S3 bucket
            using (IAmazonS3 client = new AmazonS3Client(this._awsId, this._awsKey, Amazon.RegionEndpoint.USWest2))
            {
                TransferUtility utility = new TransferUtility(client);

                using (Bitmap bitmap = new Bitmap(memoryStream))
                {
                    System.Drawing.Image thumbImage = ImageUtils.resizeImage(bitmap, padImage:false);

                    // Create a new stream instead of OpenReadStream because the stream could be closed
                    using (var thumbMemoryStream = new MemoryStream())
                    {
                        // Add the file to the memory stream
                        thumbImage.Save(thumbMemoryStream, ImageFormat.Jpeg);

                        // Create a transfer request
                        TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

                        // Create a folder with the user ID as the subdirectory
                        request.BucketName = this._awsS3Bucket + @"/" + subdir;

                        // Set the file name as the key
                        request.Key = thumbFileName;
                        request.InputStream = thumbMemoryStream;
                        request.CannedACL = S3CannedACL.PublicRead;

                        // Upload the file to S3
                        await utility.UploadAsync(request);
                    }

                }
            }
        }


        public UserImage CreateUserImage(string userId, string origImageName, string randomImageName, string thumbImageName, string subDir, string fileType)
        {
            string s3Path = string.Format("http://{0}.s3.amazonaws.com/{1}/{2}", this._awsS3Bucket, subDir, randomImageName);
            string s3ThumbPath = string.Format("http://{0}.s3.amazonaws.com/{1}/{2}", this._awsS3Bucket, subDir, thumbImageName);

            Console.Out.WriteLine(s3Path);

            // Pass the information to the database
            UserImage dbImage = new UserImage();
            dbImage.UserId = userId;
            dbImage.ImageName = randomImageName;
            dbImage.OrigImageName = origImageName;
            dbImage.S3Path = s3Path;
            dbImage.S3ThumbPath = s3ThumbPath;
            dbImage.Create = DateTime.Now;
            dbImage.Modified = DateTime.Now;
            dbImage.FileType = fileType;

            return dbImage;
        }

        /// <summary>
        /// Use Amazon Rekognition to detect the face and facial expressions.
        /// </summary>
        /// <param name="s3PhotoPath">Image path in S3 Bucket.</param>
        /// <param name="s3Bucket">S3 Bucket.</param>
        /// <returns>JSON string of the results from the image.</returns>
        public async Task<string> DetectFaces(string s3PhotoPath, string s3Bucket)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(this._awsId, this._awsKey, Amazon.RegionEndpoint.USWest2);

            DetectFacesRequest detectFacesRequest = new DetectFacesRequest()
            {
                Image = new Amazon.Rekognition.Model.Image()
                {
                    S3Object = new S3Object()
                    {
                        Name = s3PhotoPath,
                        Bucket = s3Bucket
                    },
                },
                // Attributes can be "ALL" or "DEFAULT". 
                // "DEFAULT": BoundingBox, Confidence, Landmarks, Pose, and Quality.
                // "ALL": See https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Rekognition/TFaceDetail.html
                Attributes = new List<String>() { "ALL" }
            };

            try
            {
                // Detect facial sediment
                DetectFacesResponse detectFacesResponse = await rekognitionClient.DetectFacesAsync(detectFacesRequest);

                // Display the results for debugging
                bool hasAll = detectFacesRequest.Attributes.Contains("ALL");
                foreach (FaceDetail face in detectFacesResponse.FaceDetails)
                {
                    Console.WriteLine("BoundingBox: top={0} left={1} width={2} height={3}", face.BoundingBox.Left,
                        face.BoundingBox.Top, face.BoundingBox.Width, face.BoundingBox.Height);
                    Console.WriteLine("Confidence: {0}\nLandmarks: {1}\nPose: pitch={2} roll={3} yaw={4}\nQuality: {5}",
                        face.Confidence, face.Landmarks.Count, face.Pose.Pitch,
                        face.Pose.Roll, face.Pose.Yaw, face.Quality);
                    if (hasAll)
                        Console.WriteLine("The detected face is estimated to be between " +
                            face.AgeRange.Low + " and " + face.AgeRange.High + " years old.");
                }

                return JsonConvert.SerializeObject(detectFacesResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return "";
        }

        /// <summary>
        /// Use Amazon Rekognition to detect objects.
        /// </summary>
        /// <param name="s3PhotoPath">Image path in S3 Bucket.</param>
        /// <param name="s3Bucket">S3 Bucket.</param>
        /// <returns>JSON string of the results from the image.</returns>
        public async Task<string> DetectObjects(string s3PhotoPath, string s3Bucket)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(this._awsId, this._awsKey, Amazon.RegionEndpoint.USWest2);

            DetectLabelsRequest detectlabelsRequest = new DetectLabelsRequest()
            {
                Image = new Amazon.Rekognition.Model.Image()
                {
                    S3Object = new S3Object()
                    {
                        Name = s3PhotoPath,
                        Bucket = s3Bucket
                    },
                },
                MaxLabels = 10,
                MinConfidence = 75F
            };

            try
            {
                // Get all the objects found in the image
                DetectLabelsResponse detectLabelsResponse = await rekognitionClient.DetectLabelsAsync(detectlabelsRequest);
                
                // Output for debugging
                Console.WriteLine("Detected labels for " + s3PhotoPath);
                foreach (Label label in detectLabelsResponse.Labels)
                    Console.WriteLine("{0}: {1}", label.Name, label.Confidence);


                return JsonConvert.SerializeObject(detectLabelsResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return "";
        }


        /// <summary>
        /// Use Amazon Rekognition to detect the moderation of the image if there is sexual content.
        /// </summary>
        /// <param name="s3PhotoPath">Image path in S3 Bucket.</param>
        /// <param name="s3Bucket">S3 Bucket.</param>
        /// <returns>JSON string of the results from the image.</returns>
        public async Task<string> DetectModeration(string s3PhotoPath, string s3Bucket)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(this._awsId, this._awsKey, Amazon.RegionEndpoint.USWest2);

            DetectModerationLabelsRequest detectlabelsRequest = new DetectModerationLabelsRequest()
            {
                Image = new Amazon.Rekognition.Model.Image()
                {
                    S3Object = new S3Object()
                    {
                        Name = s3PhotoPath,
                        Bucket = s3Bucket
                    },
                },
                //HumanLoopConfig = 
                MinConfidence = 75F
            };

            try
            {
                // Get all the objects found in the image
                DetectModerationLabelsResponse detectLabelsResponse = await rekognitionClient.DetectModerationLabelsAsync(detectlabelsRequest);

                // Output for debugging
                Console.WriteLine("Detected labels for " + s3PhotoPath);
                foreach (ModerationLabel label in detectLabelsResponse.ModerationLabels)
                    Console.WriteLine("{0}: {1}", label.Name, label.Confidence);


                return JsonConvert.SerializeObject(detectLabelsResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return "";
        }


        /// <summary>
        /// Use Amazon Rekognition to detect the moderation of the image if there is sexual content.
        /// </summary>
        /// <param name="s3PhotoPath">Image path in S3 Bucket.</param>
        /// <param name="s3Bucket">S3 Bucket.</param>
        /// <returns>JSON string of the results from the image.</returns>
        public async Task<string> DetectText(string s3PhotoPath, string s3Bucket)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(this._awsId, this._awsKey, Amazon.RegionEndpoint.USWest2);

            DetectTextRequest detectlabelsRequest = new DetectTextRequest()
            {
                Image = new Amazon.Rekognition.Model.Image()
                {
                    S3Object = new S3Object()
                    {
                        Name = s3PhotoPath,
                        Bucket = s3Bucket
                    },
                },
                //Filters = 
            };

            try
            {
                // Get all the objects found in the image
                DetectTextResponse detectLabelsResponse = await rekognitionClient.DetectTextAsync(detectlabelsRequest);

                // Output for debugging
                Console.WriteLine("Detected labels for " + s3PhotoPath);
                //foreach(TextDetection label in detectLabelsResponse.TextDetections)

                return JsonConvert.SerializeObject(detectLabelsResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return "";
        }
    }
}
