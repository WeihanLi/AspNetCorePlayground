using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WeihanLi.AspNetCore.Authentication.HeaderAuthentication
{
    public class HeaderAuthenticationHandler : AuthenticationHandler<HeaderAuthenticationOptions>
    {
        public HeaderAuthenticationHandler(IOptionsMonitor<HeaderAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.UserIdHeaderName))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            var userId = Request.Headers[Options.UserIdHeaderName].ToString();
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };
            if (Request.Headers.ContainsKey(Options.UserNameHeaderName))
            {
                claims.Add(new Claim(ClaimTypes.Name, Request.Headers[Options.UserNameHeaderName].ToString()));
            }
            if (Request.Headers.ContainsKey(Options.UserRolesHeaderName))
            {
                var userRoles = Request.Headers[Options.UserRolesHeaderName].ToString()
                    .Split(new[] { Options.Delimiter }, StringSplitOptions.RemoveEmptyEntries);
                claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));
            }

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

            // claims identity 's authentication type can not be null https://stackoverflow.com/questions/45261732/user-identity-isauthenticated-always-false-in-net-core-custom-authentication
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            var ticket = new AuthenticationTicket(
                principal,
                Scheme.Name
            );
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
