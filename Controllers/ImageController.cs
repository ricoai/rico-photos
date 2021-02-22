using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ricoai.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ricoai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// Configuration to get the AWS settings.
        /// </summary>
        private IConfiguration _configuration;

        /// <summary>
        /// Initializd with DI to get the configuration.
        /// </summary>
        /// <param name="configuration"></param>
        public ImageController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        // GET: api/<ImageController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ImageController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ImageController>
        [HttpPost, DisableRequestSizeLimit]
        public async void UploadFile()
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
                    bool isImage = FormFileExtensions.IsImage(file);
                    Console.Out.WriteLine("Good");

                    // Create a random file name for the file
                    // Generate a random file name
                    string randomFileName = Path.GetRandomFileName();

                    // Upload the file to S3
                    // Use the UserID as the subdirectory
                    AmazonUtils amazon = new AmazonUtils(_configuration["aws-cred:id"], _configuration["aws-cred:key"], _configuration["aws-cred:photo-bucket"]);
                    await amazon.UploadToS3(file, userId, randomFileName);
                }
            }
            catch (System.Exception ex)
            {
                Console.Out.WriteLine(ex);
            }
        }
    }
}
