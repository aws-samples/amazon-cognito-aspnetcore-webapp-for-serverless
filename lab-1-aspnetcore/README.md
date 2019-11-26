# Lab 1: Creating the AspNetCore application

The steps in this Lab will create a new AWS lambda AspNetCore to act as your main website. We will also install all the required nuget packages and add/change the code for the workshop.

## Step 1: using dotnet cli to create a new project

1. Go to **Terminal** menu and select **New Terminal**
 
<img src="../images/newterminal.png" width="500"/>

1. Configure the AWS region in your terminal by executing ```aws configure```. Provide information solely to the **Default region name**. In the example below, the *Default region name* was set to Oregon (us-west-2).
 ```
 [ec2-user@ip-172-17-2-121 WebApp]$ aws configure
 AWS Access Key ID [None]: 
 AWS Secret Access Key [None]: 
 Default region name [None]: us-west-2
 Default output format [None]: 
 ```
2. Install the AWS extensions for the dotnet CLI.
 ```
 dotnet tool install -g Amazon.Lambda.Tools
 ```
3. Install the Amazon.Lambda.Templates blueprints by executing the following command :
 ```
 dotnet new -i "Amazon.Lambda.Templates::*"
 ```
4. To see a list of the Lambda templates execute ```dotnet new lambda --list```
5. Create the *WebApp* project running the following command.

:warning: **The project name has to be named WebApp (capital W and capital A) as all the workshop modules are based on this name**.

 ```
 dotnet new serverless.AspNetCoreWebApp --name WebApp
 ```
 
6. You now have a directory called WebApp with a C# project template for AspNetCore application.

7. Go to File -> *Open* and select **/home/ec2-user/WebApp/src/WebApp/**. It will load all the sub-directories and files for you.

<img src="../images/webappfolder.png" width="500"/>

### Step 2: Publishing the serverless.AspNetCoreWebApp to AWS
Using the dotnet cli we will deploy our AspNet Core WebApp to AWS.

1. Edit the **serverless.template** file. Add the line ```"Tracing": "Active",``` right below ```"Role": null```, and add a the **AmazonSSMFullAccess** policy to the Policies: ```"Policies": [ "AWSLambdaFullAccess", "AmazonSSMFullAccess" ]```. Like the picture below. Don't forget to save the file before moving to the next step.

 <img src="../images/serverless.template.png" width="500"/>

2. Execute the following command:
 
:warning: **Note**: For the stack name use something like *your first initial* + *your last initial* + *-AspNetCoreWebApp*. For instance, John Smith: js-AspNetCoreWebApp

:notebook: **Note**: The [Support Commands Page](/SupportCommands.md) provides a list of useful commands that help you identify the names of the resources created in the labs; like the Amazon S3 bucked required for deployment.
 ```
 dotnet lambda deploy-serverless --template serverless.template --s3-bucket <bucket name> --s3-prefix "aspnetcorewebapp/" --stack-name <first initial> + <last initial> + -AspNetCoreWebApp
 ```
3. When the deployment finishes, you will be able to see all the resources that were created and the URL of your AspNetCore webapplication. Something like:
 ```
 Stack finished updating with status: CREATE_COMPLETE

 Output Name Value
 ------------------------------ --------------------------------------------------
 ApiURL https://<restApiId>.execute-api.<awsRegion>.amazonaws.com/Prod/
 ```
4. Copy and past the URL into a browser to make sure that your AspNet Core is working.
 <img src="../images/aspneturlresult.png" width="1000"/>

5. Let's check the cloudformation stack resources that were created for the AspNetCoreWebApp. The dotnet lambda blueprint took care of creating an Amazon API Gateway, IAM Roles and its policies and the Lambda itself. The following command shows the resource types and their Ids. 
```
aws cloudformation describe-stack-resources --stack-name <first initial> + <last initial> + -AspNetCoreWebApp --query 'StackResources[*].{Type:ResourceType,Id:PhysicalResourceId}' --output text
```

### Step 3: Installing the Nuget packages
Now it is time to prepare our application to integrate with the AWS resources we want by adding the AWS libraries to our project.

1. From the Visual Code terminal you will install the nuget packages, like at the screenshot below:

 <img src="../images/vscodenuget.png" width="1000"/>

:notebook: **Note:** *You can copy and paste all the lines below into the terminal prompt. The last line pasted still need to you press ENTER to install it*.

 ```
 dotnet add package Newtonsoft.Json --version 12.0.2
 dotnet add package AWSSDK.Core --version 3.3.103.48
 dotnet add package AWSSDK.Lambda --version 3.3.103.16
 dotnet add package Amazon.Lambda.Core --version 1.1.0
 dotnet add package AWSSDK.Extensions.NETCore.Setup --version 3.3.100.1
 dotnet add package AWSSDK.S3 --version 3.3.104.36
 dotnet add package AWSSDK.CognitoIdentityProvider --version 3.3.103.3
 dotnet add package AWS.Logger.Core --version 1.4.0
 dotnet add package Amazon.Lambda.APIGatewayEvents --version 1.2.0
 dotnet add package Amazon.AspNetCore.DataProtection.SSM --version 1.1.0

 ```
2. You can verify if all the packages were installed by looking at the WebApp.csproj
 ```
 cat WebApp.csproj
 ```
3. At the terminal windows, let's confirm that our code compiles with no errors
 ```
 dotnet publish -c Release
 ```

***You have now completed this lab and can move onto [Lab 2](../lab-2-openid/).***
