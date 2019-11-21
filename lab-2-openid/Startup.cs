// Copyright 2019 Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: MIT-0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Amazon.CognitoIdentityProvider;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using Amazon.S3;
using Amazon.Lambda;
using System.IO;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OpenIdConnectOptions>(Configuration.GetSection("AWS:OpenId"));
            
            var serviceProvider = services.BuildServiceProvider();
            var authOptions = serviceProvider.GetService<IOptions<OpenIdConnectOptions>>();

            services.AddDataProtection()
                .PersistKeysToAWSSystemsManager("/AspNetCoreWebApp/DataProtection");

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizePage("/Index");
                    options.Conventions.AuthorizePage("/Home");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.ResponseType = authOptions.Value.ResponseType;
                options.MetadataAddress = authOptions.Value.MetadataAddress;
                options.ClientId = authOptions.Value.ClientId;
                options.ClientSecret = authOptions.Value.ClientSecret;
                options.SaveTokens = authOptions.Value.SaveTokens;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = authOptions.Value.TokenValidationParameters.ValidateIssuer
                };
                options.Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.SetParameter("pfidpadapterid", Configuration["oidc:PingProtocolMessage"]);
                        return Task.FromResult(0);
                    },
                    // handle the logout redirection 
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        var logoutUri = Configuration["AWS:Cognito:SignedOutRedirectUri"];
                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonCognitoIdentityProvider>();
            services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonLambda>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            var loggerOptions = new LambdaLoggerOptions
            {
                IncludeCategory = false,
                IncludeLogLevel = false,
                IncludeNewline = true,

                // Configure Filter to only log some 
                Filter = (category, logLevel) =>
                {
                    // For some categories, only log events with minimum LogLevel
                    if (string.Equals(category, "Default", StringComparison.Ordinal))
                    {
                        return (logLevel >= LogLevel.Debug);
                    }
                    if (string.Equals(category, "Microsoft", StringComparison.Ordinal))
                    {
                        return (logLevel >= LogLevel.Information);
                    }
                    return true;
                }
            };

            // Configure Lambda logging
            loggerFactory
                .AddLambdaLogger(loggerOptions);

            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }

    }
}