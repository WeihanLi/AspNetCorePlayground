﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using WeihanLi.Common.Event;

namespace TestWebApplication.Event
{
    public class EventQueue
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<EventBase>> _eventQueues =
            new ConcurrentDictionary<string, ConcurrentQueue<EventBase>>();

        public ICollection<string> Queues => _eventQueues.Keys;

        public void Enqueue<TEvent>(string queueName, TEvent @event) where TEvent : EventBase
        {
            var queue = _eventQueues.GetOrAdd(queueName, q => new ConcurrentQueue<EventBase>());
            queue.Enqueue(@event);
        }

        public bool TryDequeue(string queueName, out EventBase @event)
        {
            var queue = _eventQueues.GetOrAdd(queueName, q => new ConcurrentQueue<EventBase>());
            return queue.TryDequeue(out @event);
        }

        public bool TryRemoveQueue(string queueName)
        {
            return _eventQueues.TryRemove(queueName, out _);
        }

        public bool ContainsQueue(string queueName) => _eventQueues.ContainsKey(queueName);

        public ConcurrentQueue<EventBase> this[string queueName] => _eventQueues[queueName];
    }
}
