using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net;
using RestSharp;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace CustomerList
{
    public class UiName
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string gender { get; set; }
        public string region { get; set; }
        public int age { get; set; }
        public string title { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string photo { get; set; }
        public CreditCard credit_card  { get; set; }
        public Bday birthday { get; set; }
    }

    public class Bday
    {
        public string dmy { get; set; }
        public string mdy { get; set; }
        public int raw { get; set; }                
    }

    public class CreditCard
    {
        public string expiration { get; set; }
        public string number { get; set; }
        public string pin { get; set; }
        public int security { get; set; }
    }

    public class Functions
    {
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of blogs</returns>
        public APIGatewayProxyResponse Handler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext apigProxyContext)
        {
            //Console.WriteLine($"Processing request data for request {apigProxyEvent.RequestContext.RequestId}.");
            //Console.WriteLine($"Body size = {apigProxyEvent.Body.Length}.");
            //var headerNames = string.Join(", ", apigProxyEvent.Headers.Keys);
            //Console.WriteLine($"Specified headers = {headerNames}.");
            
            try
            {
                var client = new RestClient("https://uinames.com/api/?region=canada&amount=25&ext");
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    //LambdaLogger.Log(response.Content);
                    var contents = JsonConvert.DeserializeObject<List<UiName>>(response.Content);
                    
                    return JsonResponse.Send(true, "Success", contents);
                }
                else 
                {
                    return JsonResponse.Send(false, response.ErrorMessage);
                }
            }
            catch (Exception e)
            {
                LambdaLogger.Log("Handler Error - " + e.Message);
                return JsonResponse.Send(false, e.Message);
            }
        }

        public static string Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public class BodyResponse
        {
            public string msg { get; set; }
            public IEnumerable data { get; set; }
        }

        public static class JsonResponse
        {
            public static APIGatewayProxyResponse Send(bool result, string msg, IEnumerable data = null)
            {
                BodyResponse body = new BodyResponse();
                int statusCode = 0;

                body.msg = msg;
                if (data != null)
                    body.data = data;
                else
                    body.data = null;

                if (result == false)
                {
                    statusCode = (int)HttpStatusCode.InternalServerError;
                }
                else
                {
                    statusCode = (int)HttpStatusCode.OK;
                }

                return new APIGatewayProxyResponse
                {
                    StatusCode = statusCode,
                    Headers = new Dictionary<string, string> {
                        { "Content-Type", "application/json" },
                        { "Access-Control-Allow-Origin", "*" },
                        { "Access-Control-Allow-Credentials", "true" }
                    },
                    Body = JsonConvert.SerializeObject(body)
                };

            }
        }

    }
}