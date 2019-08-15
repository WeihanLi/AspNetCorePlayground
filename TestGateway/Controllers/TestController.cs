using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TestGateway.Controllers
{
    [Route("/api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly JwtTokenOptions _tokenOptions;

        public TestController(IOptions<JwtTokenOptions> options)
        {
            _tokenOptions = options.Value;
        }

        public IActionResult Get()
        {
            return Ok(new
            {
                Tick = DateTime.UtcNow.Ticks,
                Msg = "Hello Ocelot",
            });
        }

        [HttpGet("token")]
        public IActionResult GetToken()
        {
            var now = DateTime.UtcNow;
            var claimList = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "testUser")
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: claimList,
                notBefore: now,
                expires: now.Add(_tokenOptions.TokenExpiresIn),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.SecretKey)), _tokenOptions.Algorithm));

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new
            {
                Token = token
            });
        }
    }
}
