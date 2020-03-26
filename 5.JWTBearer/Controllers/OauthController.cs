using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace _5.JWTBearer.Controllers
{
    public class OauthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type, // Authorization flow type
            string client_id, // client id
            string redirect_uri, // 
            string scope, // what info I want = email,tel,..
            string state) // random string generated to confirm that we are going back to the same client
        {
            //?a=foo&b=bar
            var query = new QueryBuilder();
            query.Add("redirect_uri", redirect_uri);
            query.Add("state", state);
            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string userName, 
            string redirect_uri, 
            string state)
        {
            const string code = "AAAAAAAAAAABBBBBB";

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);

            return Redirect($"{redirect_uri}{query.ToString()}");
        }

        public async Task<IActionResult> Token(
            string grant_type, // flow of access_token request
            string code, // Confirmation of the authentication process
            string redirect_uri,
            string client_id)
        {
            // Some machanism for validating the code



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
            var response = new
            {
                access_token = tokenJson,
                token_type = "Bearer",
                raw_claim = "oauthTurorial"
            };

            var responseJson = JsonConvert.SerializeObject(response);
            var bytes = Encoding.UTF8.GetBytes(responseJson);
            await Response.Body.WriteAsync(bytes, 0, bytes.Length);

            return Redirect(redirect_uri);
        }

    }
}
