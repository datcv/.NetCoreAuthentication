
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace _5.JWTBearer.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }


        public IActionResult Authenticate()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "any_id"),
                new Claim("RandomClaimName", "any_id")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.SecretKey));
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredential = new SigningCredentials(key, algorithm);

            var jwtToken = new JwtSecurityToken(Constants.Issurer,
                                Constants.Audiance,
                                claims,
                                notBefore: DateTime.Now,
                                expires: DateTime.Now.AddDays(1),
                                signingCredential);

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Ok(new { access_Token = tokenJson });
        }

        public IActionResult Decode(string part)
        {
            var bytes = Convert.FromBase64String(part);

            return Ok(Encoding.UTF8.GetString(bytes));
        }
    }
}
