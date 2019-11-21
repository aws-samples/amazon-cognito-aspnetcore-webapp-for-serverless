# Supporting commands
This section provides a list of useful commands that help you identify the names of the resources created in the labs. Most of the commands executed in the labs require you to know the resources, like S3 bucket and Api Gateway Id. 

## Amazon S3 bucket name
To obtain the Amazon S3 bucket name required for deploying the packages, query the resources created for the *first initial* + *last initial* + *-dotnetcore-devbox*, by executing ```aws cloudformation describe-stack-resources --stack-name <first initial> + <last initial> + -dotnetcore-devbox --query 'StackResources[*].{Type:ResourceType,Id:PhysicalResourceId}' --output text```. Look for the *AWS::S3::Bucket resource*.

## Amazon Api Gateway
You can find the **restApiId** for AspNetCoreWebApp and CustomersList by running ```aws apigateway get-rest-apis --query 'items[*].{name:name,restApiId:id}'``` and look at the name parameter.

## Amazon Cognito
List the userPools Id. Your user should be name like *first initial* + *last initial* + *-CognitoUserPool*
```
aws cognito-idp list-user-pools --max-results 10
```

Once you have the Amazon Cognito user pool Id, you can list the user-pool-clients by executing:
```
aws cognito-idp list-user-pool-clients --user-pool-id <poolId>
```

If you need to obtail the Amazon Cognito ClientId and Client Password, please execute the following command:
```
aws cognito-idp describe-user-pool-client --user-pool-id <poolId> --client-id <clientId>
```

## Lambda Functions
List all the lambda functions names that you have deploy by running:
```
aws lambda list-functions --query 'Functions[].FunctionName'
```
