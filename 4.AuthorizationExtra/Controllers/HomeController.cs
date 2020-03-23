using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _4.AuthorizationExtra.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            this._authorizationService = authorizationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy ="Claim.DoB")]
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        //[Authorize(Policy = "AdminRole")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }

        
        //[Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> SecretService()
        {
            var result = await this._authorizationService.AuthorizeAsync(this.User, "AdminRole");
            if (!result.Succeeded)
            {
                return RedirectToAction(nameof(HomeController.Authenticate));
            }

            return View("Index");
        }


        public async Task<IActionResult> SecretService2([FromServices] IAuthorizationService authorizationService)
        {
            var authorizationPolicyBuilder = new AuthorizationPolicyBuilder();
            authorizationPolicyBuilder.RequireClaim(ClaimTypes.Name, "Peter");
            var customAuthPolicy = authorizationPolicyBuilder.Build();
            var result = await authorizationService.AuthorizeAsync(this.User, customAuthPolicy);
            
            if (!result.Succeeded)
            {
                return RedirectToAction(nameof(HomeController.Authenticate));
            }

            return View("Index");
        }


        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Peter"),
                new Claim(ClaimTypes.Email, "t.wd@t.c"),
                new Claim("Random Type", "AAAAAA"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.DateOfBirth, "11223344"),
            };

            var testIdentity = new ClaimsIdentity(claims, "Test Identity");

            var licenseIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Peter K"),
                new Claim("License Type", "B"),
            }
            , "License Identity");

            var userPrincipals = new ClaimsPrincipal(new[] { testIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipals);

            return RedirectToAction(nameof(HomeController.Index));
        }
    }
}
