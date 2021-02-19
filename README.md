# Photos
This is a demo site to show my experience working with ASP.net, Angular and AWS.

The intent of this project is to upload a image.  Process the image using AI to generate some alternative images.  Allow the user to download the created results.

# What does this do
User information is stored in an SQL Server hosted on AWS RDS.

Data is received from the frontend and processed in the backend.  The image is temporarly stored on the server to verify the image.  The image is then processed using Python in using AWS Lambda.  The image and the resulting images are stored on AWS S3.  Another table keeps track of all the images created and there path.

# Who Am I
Rico Castelo
rico.castelo@gmail.com

I am in search of a new job.  My resume and profile can be found here: (https://www.linkedin.com/in/ecastelo/)[https://www.linkedin.com/in/ecastelo/]

