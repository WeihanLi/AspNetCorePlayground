using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TestWebApplication.Event
{
    public class EventConsumer : BackgroundService
    {
        private readonly EventQueue _eventQueue;
        private readonly EventStore _eventStore;
        private readonly int maxSemaphoreCount = 256;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public EventConsumer(EventQueue eventQueue, EventStore eventStore, IConfiguration configuration, ILogger<EventConsumer> logger, IServiceProvider serviceProvider)
        {
            _eventQueue = eventQueue;
            _eventStore = eventStore;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var semaphore = new SemaphoreSlim(Environment.ProcessorCount, maxSemaphoreCount))
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var queues = _eventQueue.Queues;
                    if (queues.Count > 0)
                    {
                        await Task.WhenAll(
                        queues
                            .Select(async queueName =>
                            {
                                if (!_eventQueue.ContainsQueue(queueName))
                                {
                                    return;
                                }
                                try
                                {
                                    await semaphore.WaitAsync(stoppingToken);
                                    //
                                    if (_eventQueue.TryDequeue(queueName, out var @event))
                                    {
                                        var eventHandler = _eventStore.GetEventHandler(@event, _serviceProvider);
                                        if (eventHandler is IEventHandler handler)
                                        {
                                            _logger.LogInformation(
                                                "handler {handlerType} begin to handle event {eventType}, eventId: {eventId}, eventInfo: {eventInfo}",
                                                eventHandler.GetType().FullName, @event.GetType().FullName,
                                                @event.EventId, JsonConvert.SerializeObject(@event));

                                            try
                                            {
                                                await handler.Handle(@event);
                                            }
                                            catch (Exception e)
                                            {
                                                _logger.LogError(e, "event  {eventId}  handled exception", @event.EventId);
                                            }
                                            finally
                                            {
                                                _logger.LogInformation("event {eventId} handled", @event.EventId);
                                            }
                                        }
                                        else
                                        {
                                            _logger.LogWarning(
                                                "no event handler registered for event {eventType}, eventId: {eventId}, eventInfo: {eventInfo}",
                                                @event.GetType().FullName, @event.EventId,
                                                JsonConvert.SerializeObject(@event));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "error running EventConsumer");
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            })
                    );
                    }

                    await Task.Delay(50, stoppingToken);
                }
            }
        }
    }
}
