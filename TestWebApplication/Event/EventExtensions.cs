using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace TestWebApplication.Event
{
    public static class EventExtensions
    {
        public static IServiceCollection AddEvent(this IServiceCollection services)
        {
            services.TryAddSingleton<EventStore>();
            services.TryAddSingleton<EventQueue>();
            services.TryAddSingleton<IEventPublisher, EventPublisher>();
            services.TryAddSingleton<IEventSubscriptionManager, EventSubscriptionManager>();

            services.AddSingleton<IHostedService, EventConsumer>();
            return services;
        }
    }
}
