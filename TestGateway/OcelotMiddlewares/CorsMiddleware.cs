using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Ocelot.Logging;
using Ocelot.Middleware;

namespace TestGateway.OcelotMiddlewares
{
    public class CorsMiddleware : OcelotMiddleware
    {
        private readonly GatewayOptions _gatewayOptions;
        private readonly OcelotRequestDelegate _next;

        public CorsMiddleware(OcelotRequestDelegate next, IOptions<GatewayOptions> options, IOcelotLoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<CorsMiddleware>())
        {
            _next = next;

            _gatewayOptions = options.Value;
        }

        public async Task Invoke(DownstreamContext context)
        {
            context.DownstreamResponse.Headers.Add(string.IsNullOrWhiteSpace(_gatewayOptions.AllowedOrigins)
                ? new Header(HeaderNames.AccessControlAllowOrigin, new[] { "*" })
                : new Header(HeaderNames.AccessControlAllowOrigin,
                    _gatewayOptions.AllowedOrigins.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)));

            context.DownstreamResponse.Headers.Add(new Header(HeaderNames.AccessControlAllowHeaders,
                new[] { "*" }));
            context.DownstreamResponse.Headers.Add(new Header(HeaderNames.AccessControlRequestMethod,
                new[] { "*" }));

            await _next(context);
        }
    }
}
