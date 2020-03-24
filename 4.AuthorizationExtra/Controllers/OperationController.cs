using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4.AuthorizationExtra.Controllers
{
    public class OperationController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public OperationController(IAuthorizationService authorizationService)
        {
            this._authorizationService = authorizationService;
        }

        public async Task<IActionResult> Open()
        {
            var resource = new CookieJars() { }; // Get Data from Database

            var result = await this._authorizationService.AuthorizeAsync(this.User, resource, CookieAuthRequirements.Open).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }

    public class CookieAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, CookieJars>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, CookieJars cookieJars)
        {
            if (requirement.Name == CookieJarOperations.Open)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement.Name == CookieJarOperations.DoStuff)
            {
                if (context.User.HasClaim("Friend", "Good"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }


    public static class CookieJarOperations
    {
        public const string Open = "Open";
        public const string DoStuff = "DoStuff";
    }

    public class CookieJars
    {
        public string Name { get; set; }
    }

    public class CookieAuthRequirements
    {
        public static OperationAuthorizationRequirement Open = new OperationAuthorizationRequirement()
        {
            Name = CookieJarOperations.Open
        };
    }
}
