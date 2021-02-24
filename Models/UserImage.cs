using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Date and time the image was created.
        /// </summary>
        public DateTime Create { get; set; }

        /// <summary>
        /// Date and Time the image was modified.
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Original File name.
        /// </summary>
        public string OrigImageName { get; set; }

        /// <summary>
        /// New image name.  The name is created for security and consistency.
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// Path to the image on AWS S3.
        /// </summary>
        public string S3Path { get; set; }

        /// <summary>
        /// Path to the Thumbnail image on AWS S3.
        /// </summary>
        public string S3ThumbPath { get; set; }

        /// <summary>
        /// The file type, whether it is a jpg, png or gif.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Meta Data of the image.  JSON string.
        /// </summary>
        public string MetaData { get; set; }

        /// <summary>
        /// Tags to self label the image.
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Album IDs.
        /// </summary>
        public int[] AlbumIds { get; set; }

        /// <summary>
        /// AI tags and labels.  JSON String.
        /// </summary>
        public string AiTags { get; set; }
    }
}
