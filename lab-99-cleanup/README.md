# Lab 99: Clean-up

## Step 1: Removing files from Amazon S3

:notebook: **Note**: The [Support Commands Page](/SupportCommands.md) provides a list of useful commands that help you identify the names of the resources created in the labs, as the Amazon S3 bucked required for deployment.

1. recursively deletes all objects under a bucket, for when deleting the Cloudformation Stack the bucket can be successfully deleted. If a bucket has objects, it won't get deleted.
```
aws s3 rm s3://<s3 bucket>/ --recursive
```

## Step 2: Removing Amazon Cognito
:notebook: **Note**: The [Support Commands Page](/SupportCommands.md) provides a list of useful commands that help you identify the names of the resources created in the labs; like Listing the Amazon Cognito user pools.

1. Obtain the Amazon Cognito User pool Id for the *first initial* + *last initial* + *-CognitoUserPool*.
2. Delete the user pool domain:
 ```
 aws cognito-idp delete-user-pool-domain --domain <first initial> + <last initial> + -dotnetcore --user-pool-id <poolId>
 ```
3. Delete the user pool by executing the command:
 ```
 aws cognito-idp delete-user-pool --user-pool-id <poolId>
 ```

## Step 3: Removing Cloudformation Stacks

1. Open the [CloudFormation](https://console.aws.amazon.com/cloudformation/) console.
2. Select the *first initial* + *last initial* + *-CustomerList*. Select **Delete** and confirm by clicking on **Delete stack**
3. Select the *first initial* + *last initial* + *-AspNetCoreWebApp*. Select **Delete** and confirm by clicking on **Delete stack**
4. Select the *first initial* + *last initial* + *-dotnetcore-devbox*. Select **Delete** and confirm by clicking on **Delete stack**
5. Confirm that all stacks were successfuly deleted.


**Thank you**