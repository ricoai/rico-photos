using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
                            request.Key = AmazonUtils.GenS3InternalPath(subdir, fileName);
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
                    Image thumbImage = ImageUtils.resizeImage(bitmap);

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
                        request.Key = AmazonUtils.GenS3InternalPath(subdir, thumbFileName);
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
            string s3Path = string.Format("http://{0}.s3.amazonaws.com/{1}", this._awsS3Bucket, GenS3InternalPath(subDir, randomImageName));
            string s3ThumbPath = string.Format("http://{0}.s3.amazonaws.com/{1}", this._awsS3Bucket, GenS3InternalPath(subDir, thumbImageName));

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

        public static string GenS3InternalPath(string subdir, string fileName)
        {
            return subdir + @"/" + fileName;
        }
    }
}
