﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _3.Authorization.Controllers
{
    public class HomeController : Controller
    {
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
