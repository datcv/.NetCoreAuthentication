using _3.Authorization.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace _3.Authorization
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.Cookie.Name = "TestCookie";
                    options.LoginPath = "/Home/Authenticate";
                });

            services.AddAuthorization(configue =>
            {
                //var authPolicyBuilder = new AuthorizationPolicyBuilder();
                //authPolicyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                //var defaultPolicy = authPolicyBuilder.Build();

                //configue.DefaultPolicy = defaultPolicy;


                //configue.AddPolicy("Claim.DoB", configurePolicy =>
                //{
                //    configurePolicy.RequireClaim(ClaimTypes.DateOfBirth);
                //});

                configue.AddPolicy("AdminRole", configurePolicy =>
                {
                    //configurePolicy.RequireClaim(ClaimTypes.Role, "Admin");

                    configurePolicy.AddRequirements(new CustomRequireClaim(ClaimTypes.Role));
                });

                configue.AddPolicy("Claim.DoB", configurePolicy =>
                {
                    configurePolicy.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
                });

            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
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
