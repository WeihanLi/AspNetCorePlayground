using System;
using Microsoft.AspNetCore.Authentication;

namespace WeihanLi.AspNetCore.Authentication
{
    public static class AuthenticationBuilderExtension
    {
        public static AuthenticationBuilder AddCustomHeader(this AuthenticationBuilder builder)
        {
            return builder.AddCustomHeader(HeaderAuthenticationDefaults.HeaderAuthenticationSchema);
        }

        public static AuthenticationBuilder AddCustomHeader(this AuthenticationBuilder builder, string schema)
        {
            return builder.AddCustomHeader(schema, _ => { });
        }

        public static AuthenticationBuilder AddCustomHeader(this AuthenticationBuilder builder,
            Action<HeaderAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<HeaderAuthenticationOptions, HeaderAuthenticationHandler>(HeaderAuthenticationDefaults.HeaderAuthenticationSchema,
                configureOptions);
        }

        public static AuthenticationBuilder AddCustomHeader(this AuthenticationBuilder builder, string schema,
            Action<HeaderAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<HeaderAuthenticationOptions, HeaderAuthenticationHandler>(schema,
                configureOptions);
        }
    }
}
