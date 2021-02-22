using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
        /// <param name="localFilePath">Stream for file.</param>
        /// <param name="subdir">Subdirectory within the bucket.</param>
        /// <param name="fileName">File name to use for the uploaded file.  It is suggest to make the file name random.</param>
        public async Task UploadToS3(IFormFile file, string subdir, string fileName)
        {
            try
            {
                // Create a connection to the S3 bucket
                using (IAmazonS3 client = new AmazonS3Client(this._awsId, this._awsKey, Amazon.RegionEndpoint.USWest2))
                {
                    TransferUtility utility = new TransferUtility(client);
                    TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

                    // Create a new stream instead of OpenReadStream because the stream could be closed
                    using (var newMemoryStream = new MemoryStream())
                    {
                        file.CopyTo(newMemoryStream);

                        // Create a folder with the user ID as the subdirectory
                        request.BucketName = this._awsS3Bucket + @"/" + subdir;

                        // Set the file name as the key
                        request.Key = subdir + @"/" + fileName;
                        request.InputStream = newMemoryStream;
                        request.ContentType = file.ContentType;
                        request.CannedACL = S3CannedACL.PublicRead;

                        // Upload the file to S3
                        await utility.UploadAsync(request);
                    }
                    string result = string.Format("http://{0}.s3.amazonaws.com/{1}", this._awsS3Bucket, request.Key);
                    Console.Out.WriteLine(result);
                }
            }
            catch(Exception ex)
            {
                Console.Out.WriteLine(ex);
            }
        }
    }
}
