# Installation
You need to assign values to keys and put them in either appsettings.json, appsettings.Development.json, secrets.json or Azure Application Settings.

```json 
  "aws-db": {
    "connectionString": ""
  },
  "aws-cred": {
    "id": "",
    "key": "",
    "photo-bucket": ""
  }

```

## Keys and Values
<code>aws-db:connectionString</code> is the connection string.

```json
Data Source=<xxx>.amazonaws.com,1433; Initial Catalog=<db_name>; User ID=<user>; Password=<password>;
```

<code>aws-cred:id</code> is the AWS user ID value.  Usually this is the user created in AWS, not the root user.  This user will need AWS S3, Rekognoition, and RDS permissions.<br />
<code>aws-cred:key</code> is the AWS user password key value.  User's password key.<br />
<code>aws-cred:id</code> is the AWS PhotoBucket name.<br />
