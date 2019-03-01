using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WeihanLi.AspNetCore.Authentication.QueryAuthentication
{
    public class QueryAuthenticationHandler : AuthenticationHandler<QueryAuthenticationOptions>
    {
        public QueryAuthenticationHandler(IOptionsMonitor<QueryAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Query.Count == 0)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            if (!Request.Query.ContainsKey(Options.UserIdQueryKey) || !Request.Query.ContainsKey(Options.UserNameQueryKey))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            var userId = Request.Query[Options.UserIdQueryKey].ToString();
            var userName = Request.Query[Options.UserNameQueryKey].ToString();
            var userRoles = new string[0];
            if (Request.Query.ContainsKey(Options.UserRolesQueryKey))
            {
                userRoles = Request.Query[Options.UserRolesQueryKey].ToString()
                    .Split(new[] { Options.Delimiter }, StringSplitOptions.RemoveEmptyEntries);
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
            };

            if (userRoles.Length > 0)
            {
                claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));
            }
            if (Options.AdditionalQueryToClaims.Count > 0)
            {
                foreach (var headerToClaim in Options.AdditionalQueryToClaims)
                {
                    if (Request.Query.ContainsKey(headerToClaim.Key))
                    {
                        foreach (var val in Request.Query[headerToClaim.Key].ToString().Split(new[] { Options.Delimiter }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            claims.Add(new Claim(headerToClaim.Value, val));
                        }
                    }
                }
            }
            // claims identity 's authentication type can not be null https://stackoverflow.com/questions/45261732/user-identity-isauthenticated-always-false-in-net-core-custom-authentication
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
