using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;


namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {

        private ILogger logger = null;

        public IndexModel(ILogger<DebugLogger> logger)
        {
            this.logger = logger;
        }

        public async Task OnGetAsync(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                logger.LogWarning("User not Authenticated!!!");
            }

            string accessToken = await HttpContext.GetTokenAsync("access_token");

            var JwtHandler = new JwtSecurityTokenHandler();
            var jsonToken = JwtHandler.ReadToken(accessToken) as JwtSecurityToken;
            string exp = jsonToken.Claims.FirstOrDefault(c => c.Type == "exp").Value;
            string username = jsonToken.Claims.FirstOrDefault(c => c.Type == "username").Value;
            DateTime expDate = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(Double.Parse(exp));
            string expDateStr = expDate.ToString();

            if (expDate < DateTime.Now)
            { 
                logger.LogWarning("Token Expired!!!");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                logger.LogInformation("Logging out successfuly Cookie");
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                logger.LogInformation("Logging out successfuly OpenId");
            }
                    
            ViewData["Token"] = accessToken;
            ViewData["Message"] = "Your token is valid until " + expDateStr;
        }
    }
}