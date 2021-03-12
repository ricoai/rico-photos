using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ricoai.Data;
using ricoai.Models;
using ricoai.Repositories.Interfaces;

namespace ricoai
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserImagesController : ControllerBase
    {

        /// <summary>
        /// Configuration to get the AWS settings.
        /// </summary>
        private readonly IConfiguration _configuration;

        // User Image access through repository
        private readonly IUserImagesRepository _userImageRepository;

        /// <summary>
        /// Get the UserImage Repository and configuration.
        /// The configuration is needed for AWS connection.
        /// TODO: Change from a repository to Service to move the logic of the AWS from the controller.
        /// </summary>
        /// <param name="userImageRepo">User Image Repository.</param>
        /// <param name="configuration">Configuration.</param>
        public UserImagesController(IUserImagesRepository userImageRepo, IConfiguration configuration)
        {
            //_context = context;
            _userImageRepository = userImageRepo;
            _configuration = configuration;
        }

        /// <summary>
        /// Get all the images for the specific UserID given.
        /// </summary>
        /// <param name="userId">UserID.</param>
        /// <returns>List of all the images for the given user.</returns>
        //[Route("api/images/{userId}")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserImage>>> GetAllUserImages(string userId)
        {
            //return await _context.UserImage.Where(ui => ui.UserId == userId).ToArrayAsync<UserImage>();
            return await _userImageRepository.GetAllUsersImageAsync(userId);
        }

        /// <summary>
        /// This is more used to test the database connection.  It should return something sucessfully
        /// which means a connection is made.
        /// </summary>
        /// <returns>Last 10 images.</returns>
        // GET: api/UserImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserImage>>> GetUserImage()
        {
            // Return last 10 image.
            //return await _context.UserImage.Take(10).ToListAsync<UserImage>();
            return await _userImageRepository.GetLastTenPublicAsync();
        }

        // GET: api/UserImages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserImage>> GetUserImage(int id)
        {
            var userImage = await _userImageRepository.GetByIdAsync(id);

            if (userImage == null)
            {
                return NotFound();
            }

            return userImage;
        }

        //// PUT: api/UserImages/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUserImage(int id, UserImage userImage)
        //{
        //    if (id != userImage.id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(userImage).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserImageExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/UserImages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<UserImage>> PostUserImage(UserImage userImage)
        //{
        //_context.UserImage.Add(userImage);
        //await _context.SaveChangesAsync();

        //return CreatedAtAction("GetUserImage", new { id = userImage.id }, userImage);
        //}

        /// <summary>
        /// Upload a user Image.  The Form request must include "userId" to know
        /// which user is adding an image.  Include a Form.File to upload.
        /// </summary>
        /// <returns></returns>
        // POST api/UserImages
        [HttpPost, DisableRequestSizeLimit]
        public async Task<ActionResult<UserImage>> UploadFile()
        {
            try
            {
                // Get the UserID to assoicate with the images
                Microsoft.Extensions.Primitives.StringValues userId;
                bool isUserId = Request.Form.TryGetValue("userId", out userId);

                // Get the image files
                foreach (Microsoft.AspNetCore.Http.IFormFile file in Request.Form.Files)
                {
                    //var file = Request.Form.Files[0];

                    // Verify if the given file is an actual image
                    bool isImage = ImageUtils.IsImage(file);
                    if (isImage)
                    {
                        ImageUtils.ImageDimension imgDim = ImageUtils.GetDimension(file);

                        // Create a random file name for the file
                        string randomFileName = Path.GetRandomFileName();

                        // Replace the file extension with the correct extension
                        randomFileName = Path.ChangeExtension(randomFileName, Path.GetExtension(file.FileName));

                        // Create the thumbnail image nail by appending "thumb_"
                        string thumbImageName = "thumb_" + randomFileName;

                        // Upload the file to S3
                        // Use the UserID as the subdirectory
                        AmazonUtils amazon = new AmazonUtils(_configuration["aws-cred:id"], _configuration["aws-cred:key"], _configuration["aws-cred:photo-bucket"]);
                        await amazon.UploadImageAndThumbToS3(file, userId, randomFileName, thumbImageName);

                        // Use Amazon AI to detect faces on the image
                        string jsonAiFaces = await amazon.DetectFaces(userId + @"/" + randomFileName, _configuration["aws-cred:photo-bucket"]);

                        string jsonAiObjects = await amazon.DetectObjects(userId + @"/" + randomFileName, _configuration["aws-cred:photo-bucket"]);

                        string jsonAiModeration = await amazon.DetectModeration(userId + @"/" + randomFileName, _configuration["aws-cred:photo-bucket"]);

                        string jsonAiText = await amazon.DetectText(userId + @"/" + randomFileName, _configuration["aws-cred:photo-bucket"]);

                        // Get the Meta data from the image
                        string metaJson = await ImagePropsUtil.GetProperties(file);

                        // Generate the AWS S3 paths
                        string s3Path = string.Format("http://{0}.s3.amazonaws.com/{1}/{2}", _configuration["aws-cred:photo-bucket"], userId, randomFileName);
                        string s3ThumbPath = string.Format("http://{0}.s3.amazonaws.com/{1}/{2}", _configuration["aws-cred:photo-bucket"], userId, thumbImageName);

                        // Create the UserImage object
                        UserImage usrImage = new UserImage();
                        usrImage.UserId = userId;
                        usrImage.ImageName = randomFileName;
                        usrImage.OrigImageName = file.FileName;
                        usrImage.S3Path = s3Path;
                        usrImage.S3ThumbPath = s3ThumbPath;
                        usrImage.Create = DateTime.Now;
                        usrImage.Modified = DateTime.Now;
                        usrImage.FileType = file.ContentType;
                        usrImage.Width = imgDim.Width;
                        usrImage.Height = imgDim.Height;
                        usrImage.Orientation = imgDim.Orientation;
                        usrImage.MetaData = metaJson;
                        usrImage.AiFacialTags = jsonAiFaces;
                        usrImage.AiObjectsTags = jsonAiObjects;
                        usrImage.AiModerationTags = jsonAiModeration;
                        usrImage.AiTextInImageTags = jsonAiText;
                        usrImage.FileSizeBytes = imgDim.SizeBytes;
                        usrImage.FileSizeStr = imgDim.SizeStr;

                        // Pass the information to the database
                        await _userImageRepository.InsertAsync(usrImage);

                        return CreatedAtAction("GetUserImage", new { id = usrImage.id }, usrImage);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.Out.WriteLine(ex);
            }

            return NoContent();
        }


        // DELETE: api/UserImages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserImage(int id)
        {
            if(!await _userImageRepository.Remove(id))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
