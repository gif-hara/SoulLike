using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace SoulLike
{
    public interface IMessagePublisher
    {
        void Publish<TMessage>(TMessage message);
    }

    public interface IMessageReceiver
    {
        Observable<TMessage> Receive<TMessage>();
    }

    public interface IMessageBroker : IMessagePublisher, IMessageReceiver
    {
    }

    public class MessageBroker : IMessageBroker, IDisposable
    {
        private bool isDisposed = false;

        private readonly Dictionary<Type, object> notifiers = new();

        public void Publish<TMessage>(TMessage message)
        {
            var messageType = typeof(TMessage);
            if (!notifiers.TryGetValue(messageType, out var notifier))
            {
                return;
            }
            ((Subject<TMessage>)notifier).OnNext(message);
        }

        public Observable<TMessage> Receive<TMessage>()
        {
            var messageType = typeof(TMessage);
            if (!notifiers.TryGetValue(messageType, out var notifier))
            {
                notifier = new Subject<TMessage>();
                notifiers[messageType] = notifier;
            }
            return (Observable<TMessage>)notifier;
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            foreach (var notifier in notifiers.Values)
            {
                ((IDisposable)notifier).Dispose();
            }
            notifiers.Clear();

            isDisposed = true;
        }
    }
}
