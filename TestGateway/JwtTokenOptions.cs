using System;
using Microsoft.IdentityModel.Tokens;

namespace TestGateway
{
    public class JwtTokenOptions
    {
        public TimeSpan TokenExpiresIn { get; set; } = TimeSpan.FromDays(1);

        public string SecretKey { get; set; } = "Alice@weihanli.xyz";

        public string Issuer { get; set; } = "AliceGateway";

        public string Audience { get; set; } = "wtf";

        public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    }
}
