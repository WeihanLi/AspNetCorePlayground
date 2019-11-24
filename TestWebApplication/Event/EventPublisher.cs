using System.Threading.Tasks;
using WeihanLi.Common.Event;

namespace TestWebApplication.Event
{
    public interface IEventSubscriptionManager
    {
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>;
    }

    public class EventSubscriptionManager : IEventSubscriptionManager
    {
        private readonly EventStore _eventStore;

        public EventSubscriptionManager(EventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            _eventStore.Add<TEvent, TEventHandler>();
        }
    }

    public interface IEventPublisher
    {
        Task Publish<TEvent>(string queueName, TEvent @event)
            where TEvent : EventBase;
    }

    public class EventPublisher : IEventPublisher
    {
        private readonly EventQueue _eventQueue;

        public EventPublisher(EventQueue eventQueue)
        {
            _eventQueue = eventQueue;
        }

        public Task Publish<TEvent>(string queueName, TEvent @event)
            where TEvent : EventBase
        {
            _eventQueue.Enqueue(queueName, @event);
            return Task.CompletedTask;
        }
    }
}
