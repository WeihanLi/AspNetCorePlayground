using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Event;

namespace TestWebApplication.Event
{
    public interface IEventHandler
    {
        Task Handle(IEventBase @event);
    }

    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : IEventBase
    {
        Task Handle(TEvent @event);
    }

    public class EventHandlerBase<TEvent> : IEventHandler<TEvent> where TEvent : EventBase
    {
        public virtual Task Handle(TEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Handle(IEventBase @event)
        {
            return Handle(@event as TEvent);
        }
    }

    public class EventStore
    {
        private readonly Dictionary<Type, Type> _eventHandlers = new Dictionary<Type, Type>();

        public void Add<TEvent, TEventHandler>() where TEventHandler : IEventHandler<TEvent> where TEvent : EventBase
        {
            _eventHandlers.Add(typeof(TEvent), typeof(TEventHandler));
        }

        public object GetEventHandler(Type eventType, IServiceProvider serviceProvider)
        {
            if (eventType == null || !_eventHandlers.TryGetValue(eventType, out var handlerType) || handlerType == null)
            {
                return null;
            }
            return serviceProvider.GetService(handlerType);
        }

        public object GetEventHandler(EventBase eventBase, IServiceProvider serviceProvider) =>
            GetEventHandler(eventBase.GetType(), serviceProvider);

        public object GetEventHandler<TEvent>(IServiceProvider serviceProvider) where TEvent : EventBase =>
            GetEventHandler(typeof(TEvent), serviceProvider);
    }
}
