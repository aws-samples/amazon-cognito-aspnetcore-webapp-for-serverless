// Copyright 2019 Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: MIT-0

using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CustomerList
{
    public class User
    {
        public string name { get; set; }
        public string surname { get; set; }
        public int age { get; set; }
        public string gender { get; set; }
        public string phone { get; set; }
        public string timestamp { get; set; }
    }
    
    public class Functions
    {
        static Random rnd = new Random();
        static string[] male = { "Michael","Patrick","Stefan","Daniel","Thomas","Christoph","Dominik","Lukas","Philip","Florian","Manuel","Andreas","Alexander","Markus","Martin","Matthias","Christian","Mario","Bernhard","Johannes","Maximilian","Benjamin","Raphael","Peter","Christopher","René","Simon","Marco","Fabian","Julian","Marcel","Georg","Jakob","Tobias","Clemens","Robert","Oliver","Paul","Jürgen","Wolfgang","Felix","Josef","Hannes","Roman","Gerald","Sascha","Franz","Klaus","Pascal","Roland","Richard","Gregor","Harald","Gerhard","Armin","Gabriel","Marc","Alex","Alexis","Antonio","Austin","Beau","Beckett","Bentley","Brayden","Bryce","Caden","Caleb","Camden","Cameron","Carter","Casey","Cash","Charles","Charlie","Chase","Clark","Cohen","Connor","Cooper","David","Dawson","Declan","Dominic","Drake","Drew","Dylan","Edward","Eli","Elijah","Elliot","Emerson","Emmett","Ethan","Evan","Ezra","Felix","Gage","Gavin","Gus","Harrison","Hayden","Henry","Hudson","Hunter","Isaac","Jace","Jack","Jackson","Jacob","James","Jase","Jayden","John","Jonah","Joseph","Kai","Kaiden","Kingston","Levi","Liam","Logan","Lucas","Luke","Marcus","Mason","Matthew","Morgan","Nate","Nathan","Noah","Nolan","Oliver","Owen","Parker","Raphaël","Riley","Ryan","Samuel","Sebastian","Seth","Simon","Tanner","Taylor","Theo","Tristan","Turner","Ty","William","Wyatt" };
        static string[] female = { "Julia","Lisa","Stefanie","Katharina","Melanie","Christina","Sabrina","Sarah","Anna","Sandra","Katrin","Carina","Bianca","Nicole","Jasmin","Kerstin","Tanja","Jennifer","Verena","Daniela","Theresa","Viktoria","Elisabeth","Nadine","Nina","Tamara","Madalena","Claudia","Jacquelina","Machaela","Martina","Denise","Barbara","Bettina","Alexandra","Cornelia","Maria","Vanessa","Andrea","Johanna","Eva","Natalie","Sabine","Isabella","Anja","Simone","Janine","Marlene","Patricia","Petra","Laura","Yvonne","Manuela","Karin","Birgit","Caroline","Tine","Carmen","Abigail","Adalyn","Aleah","Alexa","Alexis","Alice","Alyson","Amelia","Amy","Anabelle","Anna","Annie","Aria","Aubree","Ava","Ayla","Brielle","Brooke","Brooklyn","Callie","Camille","Casey","Charlie","Charlotte","Chloe","Claire","Danica","Elizabeth","Ella","Ellie","Elly","Emersyn","Emily","Emma","Evelyn","Felicity","Fiona","Florence","Georgia","Hailey","Haley","Isla","Jessica","Jordyn","Juliette","Kate","Katherine","Kayla","Keira","Kinsley","Kyleigh","Lauren","Layla","Lea","Leah","Lexi","Lily","Lydia","Lylah","Léa","Macie","Mackenzie","Madelyn","Madison","Maggie","Marley","Mary","Maya","Meredith","Mila","Molly","Mya","Olivia","Paige","Paisley","Peyton","Piper","Quinn","Rebekah","Rosalie","Ruby","Sadie","Samantha","Savannah","Scarlett","Selena","Serena","Sofia","Sophia","Sophie","Stella","Summer","Taylor","Tessa","Victoria","Violet","Zoey","Zoé" };
        static string[] surname = { "Gruber","Huber","Bauer","Wagner","Müller","Pichler","Steiner","Moser","Mayer","Hofer","Leitner","Berger","Fuchs","Eder","Fischer","Schmid","Winkler","Weber","Schwarz","Maier","Schneider","Reiter","Mayr","Schmidt","Wimmer","Egger","Brunner","Lang","Baumgartner","Auer","Binder","Lechner","Wolf","Wallner","Aigner","Ebner","Koller","Lehner","Haas","Schuster","Anderson","Bergeron","Bouchard","Boucher ","Brown","Bélanger","Campbell","Chan","Clark","Cote","Fortin","Gagnon","Gagné","Gauthier","Girard","Johnson","Jones","Lam","Lavoie","Lavoie","Leblanc","Lee","Li","Lévesque","Martin","Morin","Ouellet","Paquette","Patel","Pelletier","Roy","Simard","Smith","Taylor","Thompson","Tremblay","White","Williams","Wilson","Wong" };

        public User GetUser() 
        {            
            DateTime currentDate = DateTime.Now;
            var user = new User();

            // Generate random indexes for pet names.
            if (Convert.ToBoolean(rnd.Next(0, 2)))
            {
                user.name = male[rnd.Next(male.Length)];
                user.gender = "male";
            }
            else
            {
                user.name = female[rnd.Next(female.Length)];
                user.gender = "female";
            }

            user.surname = surname[rnd.Next(surname.Length)];

            user.phone = GetRandomTelNo();
            user.age = rnd.Next(21, 69);
            user.timestamp = currentDate.ToString("s");

            return user;
        }

         static string GetRandomTelNo()
         {
            StringBuilder telNo = new StringBuilder(12);
            int number;
            for (int i = 0; i < 3; i++)
            {
            number = rnd.Next(0, 8); // digit between 0 (incl) and 8 (excl)
            telNo = telNo.Append(number.ToString());
            }
            telNo = telNo.Append("-");
            number = rnd.Next(0, 743); // number between 0 (incl) and 743 (excl)
            telNo = telNo.Append(String.Format("{0:D3}", number));
            telNo = telNo.Append("-");
            number = rnd.Next(0, 10000); // number between 0 (incl) and 10000 (excl)
            telNo = telNo.Append(String.Format("{0:D4}", number));
            return telNo.ToString();                  
         }

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
            try
            {
                var users = new List<User>();

                int counter = 0;
                while (counter < 25)
                {
                    users.Add(GetUser());
                    counter++;
                } 

                return JsonResponse.Send(true, "Success", users);
                               
            }
            catch (Exception e)
            {
                LambdaLogger.Log("Handler Error - " + e.Message);
                return JsonResponse.Send(false, e.Message);
            }
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