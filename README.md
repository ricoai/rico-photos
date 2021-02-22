# Photos
This is a demo site to show my experience working with ASP.net, Angular and AWS.

The intent of this project is to upload a image.  Process the image using AI to generate some alternative images.  Allow the user to download the created results.

# What does this do
User information is stored in an SQL Server hosted on AWS RDS.

Data is received from the frontend and processed in the backend.  The image is temporarly stored on the server to verify the image.  The image is then processed using Python in using AWS Lambda.  The image and the resulting images are stored on AWS S3.  Another table keeps track of all the images created and there path.

# Who Am I
Rico Castelo
rico.castelo@gmail.com

I am in search of a new job.  My resume and profile can be found here: [https://www.linkedin.com/in/ecastelo/](https://www.linkedin.com/in/ecastelo/)



# Image Upload
The image is uploaded to the server.  The server then verifies it is an image.  If the image is verified, it will continue processing.

## Verify Image
How do I verify it is an image.  I verify the image on the backend in ASP.net.  
* Check its mime type.  I verify the ContentType is image/jpg, image/jpeg, image/pjpeg, image/gif, image/x-png, or image/pngl
* Check the file extension.  Verify it is a  .jpg, .png, .gif, .jpeg.
* Read in the first couple bytes and verify it is not harmful.
* Open the file as a bitmap.  If when opening the file it throws an exception, then I cannot trust the file.

This is all done in the Models/FileVerify file.  The code was found at: [https://stackoverflow.com/a/51879881/1380462](https://stackoverflow.com/a/51879881/1380462c)


