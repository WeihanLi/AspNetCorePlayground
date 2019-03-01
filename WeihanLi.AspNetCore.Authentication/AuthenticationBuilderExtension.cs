using System;
using Microsoft.AspNetCore.Authentication;
using WeihanLi.AspNetCore.Authentication.HeaderAuthentication;
using WeihanLi.AspNetCore.Authentication.QueryAuthentication;

namespace WeihanLi.AspNetCore.Authentication
{
    public static class AuthenticationBuilderExtension
    {
        #region AddHeader

        public static AuthenticationBuilder AddHeader(this AuthenticationBuilder builder)
        {
            return builder.AddHeader(HeaderAuthenticationDefaults.AuthenticationSchema);
        }

        public static AuthenticationBuilder AddHeader(this AuthenticationBuilder builder, string schema)
        {
            return builder.AddHeader(schema, _ => { });
        }

        public static AuthenticationBuilder AddHeader(this AuthenticationBuilder builder,
            Action<HeaderAuthenticationOptions> configureOptions)
        {
            return builder.AddHeader(HeaderAuthenticationDefaults.AuthenticationSchema,
                configureOptions);
        }

        public static AuthenticationBuilder AddHeader(this AuthenticationBuilder builder, string schema,
            Action<HeaderAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<HeaderAuthenticationOptions, HeaderAuthenticationHandler>(schema,
                configureOptions);
        }

        #endregion AddHeader

        #region AddQuery

        public static AuthenticationBuilder AddQuery(this AuthenticationBuilder builder)
        {
            return builder.AddHeader(HeaderAuthenticationDefaults.AuthenticationSchema);
        }

        public static AuthenticationBuilder AddQuery(this AuthenticationBuilder builder, string schema)
        {
            return builder.AddQuery(schema, _ => { });
        }

        public static AuthenticationBuilder AddQuery(this AuthenticationBuilder builder,
            Action<QueryAuthenticationOptions> configureOptions)
        {
            return builder.AddQuery(HeaderAuthenticationDefaults.AuthenticationSchema,
                configureOptions);
        }

        public static AuthenticationBuilder AddQuery(this AuthenticationBuilder builder, string schema,
            Action<QueryAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<QueryAuthenticationOptions, QueryAuthenticationHandler>(schema,
                configureOptions);
        }

        #endregion AddQuery
    }
}
