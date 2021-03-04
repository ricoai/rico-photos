# Photos
This is a demo site to show my experience working with ASP.net, Angular and AWS.

The intent of this project is to upload a image.  Process the image using AI determine the semantics of the image.  Display all the details of the image.

![rico.ai Photos Example](https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-ad.gif)

# What does this do
User information is stored in an SQL Server hosted on AWS RDS.

Images are stored in AWS S3 Bucket.  Each user has there own folder.  The original image and a thumbnail are stored.

The image is then processed by AWS Rekognition to determine the semantics of the image.
 * Determine if there are any objects in the images. (Object Detection)
 * Determine if there are any faces and the there facial expressions. (Facial Expressions and Sediment)
 * Determine if there are any sexual content. (Moderation)
 * Determine if there is any text.  (Text Detection)


 Meta data of the image is obtained to determine information like location where the image was taken, the settings and camera type.

Frontend uses Angular to upload and display the images.  The uploaded images are displayed in a grid of thumbnails.  Each thumbnail can be clicked to see all the AI results of the iamge.

The SQL Server keeps track of all the AI results and the S3 paths for each image.

# Who Am I
Rico Castelo
rico.castelo@gmail.com

I am in search of a new job.  My resume and profile can be found here: [https://www.linkedin.com/in/ecastelo/](https://www.linkedin.com/in/ecastelo/)


# Installation
* Check out the code.  
* Right click on the project and select "Manager User Secrets" and copy the lines below.
```json
{
  "aws-db": {
    "connectionString": "Data Source=***.***.us-west-2.rds.amazonaws.com,1433; Initial Catalog=***; User ID=***; Password=***;"
  },
  "aws-cred": {
    "id": "***",
    "key": "***",
    "photo-bucket": "***",
  }
}
```
Anywhere there is <code>***</code> enter in your AWS settings.

You will need to create the permissions for your user to allow AWS S3 GET and PUT and AWS Reckongition.
* In "Package Manager Console" send the command 
```bash
Add-Migration Init -Context RicoaiDbContext
Update-Database -Context RicoaiDbContext
```


# Image Upload
The image is uploaded to the server.  The server then verifies it is an image.  If the image is verified, it will continue processing.

## Verify Image
How do I verify it is an image.  I verify the image on the backend in ASP.net.  
* Check its mime type.  I verify the ContentType is image/jpg, image/jpeg, image/pjpeg, image/gif, image/x-png, or image/pngl
* Check the file extension.  Verify it is a  .jpg, .png, .gif, .jpeg.
* Read in the first couple bytes and verify it is not harmful.
* Open the file as a bitmap.  If when opening the file it throws an exception, then I cannot trust the file.

This is all done in the Models/ImageUtils.cs file.  The code was found at: [https://stackoverflow.com/a/51879881/1380462](https://stackoverflow.com/a/51879881/1380462c)



# Thumbnail
A thumbnail of the image is created.  The thumbnail is set to the size 1280x720.  The thumbnail image is also determined what orientation to keep consistency.

This is done in Models/ImageUtils.cs file.


# Meta Data
Meta data is obtained for each image.  This Meta data is stored in the AWS RDS database.  Each image will vary on what meta data is available.  The meta data is decoded and stored as JSON to see all the available meta data.

The meta data can include the GPS location, camera settings and the type of camera used. 

This is done in Models/ImagePropsUtils.cs file.


# AI Image Semantics
The image is then scanned by a couple different models to determine the semantics of the image.  This includes object detection, text detection, facial sedmient and scan for mature content.

The models are all supplied by AWS Rekognition.  The backend utilizes the Amazon.Rekognition from Package Manager to upload the image to the service and get a response back containing the information of the file.  

This is done in Models/AmazonUtils.cs file.