using Microsoft.AspNetCore.Authentication;
using System;

namespace WeihanLi.AspNetCore.Authentication
{
    public static class AuthenticationBuilderExtension
    {
        public static AuthenticationBuilder AddCustomHeader(this AuthenticationBuilder builder)
        {
            return builder.AddCustomHeader(_ => { });
        }

        public static AuthenticationBuilder AddCustomHeader(this AuthenticationBuilder builder,
            Action<HeaderAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<HeaderAuthenticationOptions, HeaderAuthenticationHandler>(HeaderAuthenticationDefaults.HeaderAuthenticationSchema, "HeaderAuthentication",
                configureOptions);
        }
    }
}