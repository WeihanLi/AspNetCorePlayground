using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WeihanLi.AspNetCore.Authentication
{
    public class HeaderAuthenticationHandler : AuthenticationHandler<HeaderAuthenticationOptions>
    {
        public HeaderAuthenticationHandler(IOptionsMonitor<HeaderAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.UserIdHeaderName) || !Request.Headers.ContainsKey(Options.UserNameHeaderName))
            {
                return Task.FromResult(AuthenticateResult.Fail("unauthorized"));
            }
            var userId = Request.Headers[Options.UserIdHeaderName].ToString();

            var userName = Request.Headers[Options.UserNameHeaderName].ToString();
            var userRoles = new string[0];
            if (Request.Headers.ContainsKey(Options.UserRolesHeaderName))
            {
                userRoles = Request.Headers[Options.UserRolesHeaderName].ToString()
                    .Split(new[] { Options.Delimiter }, StringSplitOptions.RemoveEmptyEntries);
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
            };
            claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            if (Options.AdditionalHeaderToClaims.Count > 0)
            {
                foreach (var headerToClaim in Options.AdditionalHeaderToClaims)
                {
                    if (Request.Headers.ContainsKey(headerToClaim.Key))
                    {
                        foreach (var val in Request.Headers[headerToClaim.Key].ToString().Split(new[] { Options.Delimiter }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            claims.Add(new Claim(headerToClaim.Value, val));
                        }
                    }
                }
            }

            return Task.FromResult(
                AuthenticateResult.Success(new AuthenticationTicket(
                    new ClaimsPrincipal(new ClaimsIdentity(claims)),
                    HeaderAuthenticationDefaults.HeaderAuthenticationSchema
                ))
            );
        }
    }
}