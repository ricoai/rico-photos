using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ricoai.Models
{
    public class UserImage
    {
        /// <summary>
        /// ID of the image.
        /// </summary>
        public int id { get; set; }


        /// <summary>
        ///  User ID.  Owner of the file
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// Original File name.
        /// </summary>
        public string origImageName { get; set; }

        /// <summary>
        /// New image name.  The name is created for security and consistency.
        /// </summary>
        public string imageName { get; set; }

        /// <summary>
        /// Path to the AWS S3 Path.
        /// </summary>
        public string s3Path { get; set; }
    }
}
