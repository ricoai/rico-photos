using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ricoai.Models
{
    /// <summary>
    /// Used to verify if the given form data is an image or another type of file.
    /// This is to prevent malicious files from being uploaded.
    /// </summary>
    public static class ImageUtils
    {
        public class ImageDimension
        {
            public int Width { get; set; }
            public int Height { get; set; }

            /// <summary>
            /// Orientation
            /// 0 = Landscape
            /// 1 = Portrait
            /// 2 = Square
            /// </summary>
            public short Orientation { get; set; }
        }

        /// <summary>
        /// Minimum number of bytes an image should have to verify the image is good.
        /// </summary>
        public const int ImageMinimumBytes = 512;

        //set the resolution, 72 is usually good enough for displaying images on monitors
        public const float imageResolution = 72;

        //set the compression level. higher compression = better quality = bigger images
        public const long compressionLevel = 80L;

        /// <summary>
        /// Given a file from a form, verify if it is an image file.
        /// This will check the file extension and also try to open the file.  
        /// This is to prevent malicious files from being uploaded.
        /// 
        /// Code found here:
        /// https://stackoverflow.com/questions/11063900/determine-if-uploaded-file-is-image-any-format-on-mvc
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static bool IsImage(this IFormFile postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return false;
                }
                //------------------------------------------
                //check whether the image size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.Length < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                postedFile.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using var bitmap = new Bitmap(postedFile.OpenReadStream());
                ;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.OpenReadStream().Position = 0;
            }

            return true;
        }

        /// <summary>
        /// Get the image dimension and orientation.
        /// This will get the image width, height and orientation.
        /// 
        /// Orientation:
        /// Landscape = 0
        /// Portrait = 1
        /// Square = 2
        /// </summary>
        /// <param name="postedFile">IFormFile from the form from the API.</param>
        /// <returns></returns>
        public static ImageDimension GetDimension(this IFormFile postedFile)
        {
            ImageDimension imgDim = new ImageDimension();

            try
            {
                // Open a file stream to the image
                var bitmap = new Bitmap(postedFile.OpenReadStream());

                // Get the length and width
                imgDim.Width = bitmap.Width;
                imgDim.Height = bitmap.Height;


                //first we check if the image needs rotating (eg phone held vertical when taking a picture for example)
                foreach (var prop in bitmap.PropertyItems)
                {
                    if (prop.Id == 0x0112)
                    {
                        int orientationValue = bitmap.GetPropertyItem(prop.Id).Value[0];
                        switch(orientationValue)
                        { 
                            case 6:
                            case 8:
                                // Portrait
                                imgDim.Orientation = 1;
                            break;
                            case 1:
                                // Landscape
                                imgDim.Orientation = 0;
                                break;
                            default:
                                // Default to portrait
                                imgDim.Orientation = 1;
                            break;
                        }  
                    }
                }

                // THIS DID NOT WORK FOR CELL PHONE IMAGES
                // DIMENSIONS WERE BASICALLY NOT CHANGING BASED ON ORIENTATION
                /**
                // Get the orientation
                if (bitmap.Width > bitmap.Height)
                {
                    // Landscape
                    imgDim.Orientation = 0;
                }
                else if (bitmap.Width < bitmap.Height)
                {
                    // Portrait
                    imgDim.Orientation = 1;
                }
                else
                {
                    // Square
                    imgDim.Orientation = 3;
                }
                */

            }
            catch (Exception)
            {
                return imgDim;
            }
            finally
            {
                postedFile.OpenReadStream().Position = 0;
            }

            return imgDim;
        }

        /// <summary>
        /// Resize the image to craete a thumbnail.
        /// 
        /// https://stackoverflow.com/a/46838111/1380462
        /// </summary>
        /// <param name="image">Image to resize.</param>
        /// <param name="maxWidth">Maximum width of the image.  Thumbnail 1280.</param>
        /// <param name="maxHeight">Maximum height of the image.  Thumbnail 720.</param>
        /// <param name="padImage">Add padding around images that are taken vertically.</param>
        /// <returns></returns>
        public static Image resizeImage(Image image, int maxWidth = 1280, int maxHeight=720, bool padImage=true)
        {
            int newWidth;
            int newHeight;

            //first we check if the image needs rotating (eg phone held vertical when taking a picture for example)
            foreach (var prop in image.PropertyItems)
            {
                if (prop.Id == 0x0112)
                {
                    int orientationValue = image.GetPropertyItem(prop.Id).Value[0];
                    RotateFlipType rotateFlipType = getRotateFlipType(orientationValue);
                    image.RotateFlip(rotateFlipType);
                    break;
                }
            }

            //apply the padding to make a square image
            if (padImage == true)
            {
                image = applyPaddingToImage(image, Color.Transparent);
            }

            //check if the with or height of the image exceeds the maximum specified, if so calculate the new dimensions
            if (image.Width > maxWidth || image.Height > maxHeight)
            {
                double ratioX = (double)maxWidth / image.Width;
                double ratioY = (double)maxHeight / image.Height;
                double ratio = Math.Min(ratioX, ratioY);

                newWidth = (int)(image.Width * ratio);
                newHeight = (int)(image.Height * ratio);
            }
            else
            {
                newWidth = image.Width;
                newHeight = image.Height;
            }

            //start the resize with a new image
            Bitmap newImage = new Bitmap(newWidth, newHeight);

            //set the new resolution
            newImage.SetResolution(imageResolution, imageResolution);

            //start the resizing
            using (var graphics = Graphics.FromImage(newImage))
            {
                //set some encoding specs
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            //save the image to a memorystream to apply the compression level
            using (MemoryStream ms = new MemoryStream())
            {
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, compressionLevel);

                newImage.Save(ms, getEncoderInfo("image/jpeg"), encoderParameters);

                //save the image as byte array here if you want the return type to be a Byte Array instead of Image
                //byte[] imageAsByteArray = ms.ToArray();
            }

            //return the image
            return newImage;
        }


        /// <summary>
        /// Add Image padding if the image was taken vertically.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backColor"></param>
        /// <returns></returns>
        public static Image applyPaddingToImage(Image image, Color backColor)
        {
            //get the maximum size of the image dimensions
            int maxSize = Math.Max(image.Height, image.Width);
            Size squareSize = new Size(maxSize, maxSize);

            //create a new square image
            Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);

            using (Graphics graphics = Graphics.FromImage(squareImage))
            {
                //fill the new square with a color
                graphics.FillRectangle(new SolidBrush(backColor), 0, 0, squareSize.Width, squareSize.Height);

                //put the original image on top of the new square
                graphics.DrawImage(image, (squareSize.Width / 2) - (image.Width / 2), (squareSize.Height / 2) - (image.Height / 2), image.Width, image.Height);
            }

            //return the image
            return squareImage;
        }


        /// <summary>
        /// Get the encoder information of the image.
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo getEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            for (int j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType.ToLower() == mimeType.ToLower())
                {
                    return encoders[j];
                }
            }

            return null;
        }


        /// <summary>
        /// Determine if a rotation has been done to the image.
        /// </summary>
        /// <param name="rotateValue"></param>
        /// <returns></returns>
        private static RotateFlipType getRotateFlipType(int rotateValue)
        {
            RotateFlipType flipType = RotateFlipType.RotateNoneFlipNone;

            switch (rotateValue)
            {
                case 1:
                    flipType = RotateFlipType.RotateNoneFlipNone;
                    break;
                case 2:
                    flipType = RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    flipType = RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    flipType = RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    flipType = RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    flipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    flipType = RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    flipType = RotateFlipType.Rotate270FlipNone;
                    break;
                default:
                    flipType = RotateFlipType.RotateNoneFlipNone;
                    break;
            }

            return flipType;
        }


        /// <summary>
        /// Convert the image to Base64.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string convertImageToBase64(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //convert the image to byte array
                image.Save(ms, ImageFormat.Jpeg);
                byte[] bin = ms.ToArray();

                //convert byte array to base64 string
                return Convert.ToBase64String(bin);
            }
        }

        /// <summary>
        /// Check the orientation of the image.  Is it Portrait, Landscape or Square.
        /// </summary>
        /// <param name="image">Image to detect.</param>
        /// <returns>0 = Landscape, 1 = Portrait orientation, 2 = Square</returns>
        public static int Orientation(Image image)
        {
            if(image.Width > image.Height)
            {
                // Landscape
                return 0;
            }
            else if(image.Width < image.Height)
            {
                // Portrait
                return 1;
            }

            // Square
            return 3;
        }

        /// <summary>
        /// Get the Meta data from the image.  Then return the data as a JSON string.
        /// </summary>
        /// <param name="file">Image file to get the data.</param>
        /// <returns>JSON string of the image meta data.</returns>
        public static async Task<string> GetMetaData(IFormFile file)
        {
            var dict = new Dictionary<string, string>();

            try
            {
                using (var imageMemoryStream = new MemoryStream())
                {

                    // Add the file to the memory stream
                    await file.CopyToAsync(imageMemoryStream);

                    using (Bitmap bitmap = new Bitmap(imageMemoryStream))
                    {
                        // Get the PropertyItems property from image.
                        PropertyItem[] propItems = bitmap.PropertyItems;

                        foreach(PropertyItem propItem in propItems)
                        {
                            // ASCII
                            if (propItem.Type == 2)
                            {
                                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                                dict.Add("0x" + propItem.Id.ToString("X4"), encoding.GetString(propItem.Value));
                            }
                            else if(propItem.Type == 3)
                            {
                                dict.Add("0x" + propItem.Id.ToString("X4"), BitConverter.ToInt16(propItem.Value).ToString());
                            }
                            else if (propItem.Type == 4)
                            {
                                dict.Add("0x" + propItem.Id.ToString("X4"), BitConverter.ToInt32(propItem.Value).ToString());
                            }
                            else
                            {
                                dict.Add("0x" + propItem.Id.ToString("X4"), propItem.Value.ToString());
                            }
                        }
                    }

                }
            } catch(Exception ex)
            {
                Console.Out.WriteLine(ex);
            }

            return JsonConvert.SerializeObject(dict);
        }
    }
}
