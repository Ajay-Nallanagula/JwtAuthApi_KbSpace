using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWTAuthenticationApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace JWTAuthenticationApi.Controllers
{
    [Produces("application/json")]
    [Route("api/JsonWebToken")]
    public class JsonWebTokenController : Controller
    {
        public JsonWebTokenController(IConfiguration config)
        {
            ConfigurationBinder = config;
        }

        public IConfiguration ConfigurationBinder { get; }

        [Route("CreateToken")]
        [AllowAnonymous,HttpPost]
        public IActionResult CreateToken([FromBody]LoginModel loginModel)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(loginModel);
            if (user != null)
            {
                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        private object BuildToken(UserModel user)
        {
            var claimsArr = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Name),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Birthdate,user.Birthdate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti,(new Guid()).ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationBinder["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: ConfigurationBinder["Jwt:Issuer"],
                audience: ConfigurationBinder["Jwt:Issuer"],
                claims: claimsArr,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            return jwtTokenHandler.WriteToken(token);
        }


        private UserModel AuthenticateUser(LoginModel loginModel)
        {
            UserModel user = null;
            if (loginModel.Username.Equals("Ajay@gmail.com") && loginModel.Password.Equals("Nallanagula"))
            {
                user = new UserModel { Name = "Ajay Nallanagula", Email = "ajay.nallanagula@domain.com", Birthdate = new DateTime(1986, 2, 9) };
            }
            return user;
        }
    }

}