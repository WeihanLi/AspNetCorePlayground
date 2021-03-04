using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using WeihanLi.Extensions;

namespace OcelotDemo
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddCustomJwt(this AuthenticationBuilder builder, string schema, IConfiguration configuration)
        {
            IdentityModelEventSource.ShowPII = configuration[nameof(IdentityModelEventSource.ShowPII)].ToOrDefault<bool>();
            return builder.AddJwtBearer(schema, configuration.Bind);
        }
    }
}
