using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Ocelot.Authorization;
using Ocelot.Logging;
using Ocelot.Middleware;

namespace OcelotDemo.OcelotMiddleware
{
    public class ApiPermission
    {
        public string AllowedRoles { get; set; }

        public string PathPattern { get; set; }

        public string Method { get; set; }
    }

    public class UrlBasedAuthenticationMiddleware : Ocelot.Middleware.OcelotMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly RequestDelegate _next;

        public UrlBasedAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, IMemoryCache memoryCache, IOcelotLoggerFactory loggerFactory) : base(loggerFactory.CreateLogger<UrlBasedAuthenticationMiddleware>())
        {
            _next = next;

            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // var permissions = await _memoryCache.GetOrCreateAsync("ApiPermissions", async entry =>
            //{
            //    using (var conn = new SqlConnection(_configuration.GetConnectionString("ApiPermissions")))
            //    {
            //        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            //        return (await conn.QueryAsync<ApiPermission>("SELECT * FROM dbo.ApiPermissions")).ToArray();
            //    }
            //});

            var permissions = new[]
            {
               new ApiPermission()
               {
                   PathPattern = "/api/test/values",
                   Method = "GET",
                   AllowedRoles = ""
               },
               new ApiPermission()
               {
                   PathPattern = "/api/test/user",
                   Method = "GET",
                   AllowedRoles = "User"
               },
               new ApiPermission()
               {
                   PathPattern = "/api/test/admin",
                   Method = "GET",
                   AllowedRoles = "Admin"
               },
            };

            var downstreamRoute = httpContext.Items.DownstreamRoute();

            var result = await httpContext.AuthenticateAsync(downstreamRoute.AuthenticationOptions.AuthenticationProviderKey);
            if (result.Principal != null)
            {
                httpContext.User = result.Principal;
            }

            var user = httpContext.User;
            var request = httpContext.Request;

            var permission = permissions.FirstOrDefault(p =>
                request.Path.ToString().Equals(p.PathPattern, StringComparison.OrdinalIgnoreCase) && p.Method.ToUpper() == request.Method.ToUpper());

            if (permission == null)// 完全匹配不到，再根据正则匹配
            {
                permission =
                    permissions.FirstOrDefault(p =>
                        Regex.IsMatch(request.Path.ToString(), p.PathPattern, RegexOptions.IgnoreCase) && p.Method.ToUpper() == request.Method.ToUpper());
            }

            if (user.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrWhiteSpace(permission?.AllowedRoles) &&
                    !permission.AllowedRoles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Any(r => user.IsInRole(r)))
                {
                    httpContext.Items.SetError(new UnauthorizedError("forbidden, have no permission"));
                    return;
                }
            }
            else
            {
                if (permission != null && string.IsNullOrWhiteSpace(permission.AllowedRoles))
                {
                }
                else
                {
                    httpContext.Items.SetError(new UnauthenticatedError("unauthorized, need login"));
                    return;
                }
            }

            await _next.Invoke(httpContext);
        }
    }
}
