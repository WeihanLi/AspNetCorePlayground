using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeihanLi.Common.Event;

namespace TestWebApplication.Event
{
    public class PageViewEvent : EventBase
    {
        public string Path { get; set; }
    }

    public class PageViewEventHandler : EventHandlerBase<PageViewEvent>
    {
        private readonly ILogger _logger;

        public PageViewEventHandler(ILogger<PageViewEventHandler> logger)
        {
            _logger = logger;
        }

        public override Task Handle(PageViewEvent @event)
        {
            _logger.LogInformation($"handle pageViewEvent: {JsonConvert.SerializeObject(@event)}");
            return Task.CompletedTask;
        }
    }
}
