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
        /// Set whether this image is public which means anyone can view the image and will
        /// be visiable on the main page.
        /// </summary>
        [Required]
        public bool IsPublic { get; set; }

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
        /// Width of the original image in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the original image in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Get the orientation of the image.
        /// Landscape = 0
        /// Portrait = 1
        /// Square = 2
        /// </summary>
        public int Orientation { get; set; }

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
        /// Tags to self label the image.  JSON String.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Album IDs.
        /// </summary>
        //public int[] AlbumIds { get; set; }

        /// <summary>
        /// AI Objects tags and labels.  This will find any labels for objects in the image.  JSON String.
        /// </summary>
        public string AiObjectsTags { get; set; }

        /// <summary>
        /// AI Facial Analysis tags and labels.  This will find any labels for facial experiences and sediment. JSON String.
        /// </summary>
        public string AiFacialTags { get; set; }

        /// <summary>
        /// AI Image Moderation tags and labels.  This will determine if the file has any sexual situations.  JSON String.
        /// </summary>
        public string AiModerationTags { get; set; }

        /// <summary>
        /// AI Text in Image.  This will determine if any text is in the image.  JSON String.
        /// </summary>
        public string AiTextInImageTags { get; set; }
    }
}
