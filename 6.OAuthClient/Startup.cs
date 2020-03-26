using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

/*
 https://tools.ietf.org/html/rfc6749
 +----------+
     | Resource |
     |   Owner  |
     |          |
     +----------+
          ^
          |
         (B)
     +----|-----+          Client Identifier      +---------------+
     |         -+----(A)-- & Redirection URI ---->|               |
     |  User-   |                                 | Authorization |
     |  Agent  -+----(B)-- User authenticates --->|     Server    |
     |          |                                 |               |
     |         -+----(C)-- Authorization Code ---<|               |
     +-|----|---+                                 +---------------+
       |    |                                         ^      v
      (A)  (C)                                        |      |
       |    |                                         |      |
       ^    v                                         |      |
     +---------+                                      |      |
     |         |>---(D)-- Authorization Code ---------'      |
     |  Client |          & Redirection URI                  |
     |         |                                             |
     |         |<---(E)----- Access Token -------------------'
     +---------+       (w/ Optional Refresh Token)

   Note: The lines illustrating steps(A), (B), and(C) are broken into
 two parts as they pass through the user-agent.

                   Figure 3: Authorization Code Flow

*/
namespace _6.OAuthClient
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                // Check the cookie to confirm that we are authenticated
                config.DefaultAuthenticateScheme = "ClientCookie";

                // When we sign in we will deal out a cookie
                config.DefaultSignInScheme = "ClientCookie";

                // Use this to check if we allowed to do something
                config.DefaultChallengeScheme = "OirServer";
            })
            .AddCookie("ClientCookie", config =>
            {
                config.Cookie.Name = "ClientOAuthCookie";
            })
            .AddOAuth("OirServer", config =>
            {
                config.ClientId = "client_id";
                config.ClientSecret = "client_secret";
                config.CallbackPath = "/oauth/callback";
                config.AuthorizationEndpoint = "https://localhost:44373/oauth/authorize";
                config.TokenEndpoint = "https://localhost:44373/oauth/token";

                config.SaveTokens = true;

                config.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents()
                {
                    OnCreatingTicket = context =>
                    {
                        var accessToken = context.AccessToken;
                        var payload = accessToken.Split('.')[1];

                        // Repad the payload to make sure it divisibke by 4
                        switch (payload.Length % 4)
                        {
                            case 1:
                                payload += "===";
                                break;
                            case 2:
                                payload += "==";
                                break;
                            case 3:
                                payload += "=";
                                break;
                        }


                        var bytes = Convert.FromBase64String(payload);
                        var jsonPayload = Encoding.UTF8.GetString(bytes);
                        var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                        foreach (var claim in claims)
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim(claim.Key, claim.Value));
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
