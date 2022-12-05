using System;
using System.Collections.Generic;

namespace DimenshipBase.Broker;

public interface IMessage
{
}

public interface IBroker
{
    void Publish(IMessage message);
    IBroker Subscribe(Action<IMessage> handler, string messageClass, string subscriberId);
    IBroker Subscribe<T>(Action<T> handler, string subscriberId) where T:IMessage;
}

public interface IHandlerWrapper
{
    void Handle(IMessage m);
}

public class HandlerWrapper<T>:IHandlerWrapper where T:IMessage
{
    private readonly Action<T> _handler;
    private readonly string _subscriberId;

    public HandlerWrapper(Action<T> handler, string subscriberId)
    {
        _handler = handler;
        _subscriberId = subscriberId;
    }

    public void Handle(IMessage m)
    {
        try
        {
            _handler.Invoke((T)m);
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"Run into exception handling <{typeof(T)}> {m?.ToString()} message for {_subscriberId}");
        }
    }
}

public class SimpleBroker : IBroker
{
    private readonly object _syncRoot = new object();
    public readonly Dictionary<string, List<IHandlerWrapper>> _handlers = new Dictionary<string, List<IHandlerWrapper>>();
    public void Publish(IMessage message)
    {
        lock (_syncRoot)
        {
            string messageClass = message.GetType().Name;
            if (_handlers.TryGetValue(messageClass, out var subscribers))
            {
                foreach (var subscriber in subscribers)
                {
                    subscriber.Handle(message);
                }
            }
        }
    }

    public IBroker Subscribe(Action<IMessage> handler, string messageClass, string subscriberId)
    {
        lock (_syncRoot)
        {
            List<IHandlerWrapper> subscribers;
            if (!_handlers.TryGetValue(messageClass, out subscribers))
            {
                subscribers = new List<IHandlerWrapper>();
                _handlers.Add(messageClass, subscribers);
            }
            subscribers.Add(new HandlerWrapper<IMessage>(handler, subscriberId));
        }

        return this;
    }

    public IBroker Subscribe<T>(Action<T> handler, string subscriberId) where T : IMessage
    {
        lock (_syncRoot)
        {
            List<IHandlerWrapper> subscribers;
            string messageClass =typeof(T).Name;
            if (!_handlers.TryGetValue(messageClass, out subscribers))
            {
                subscribers = new List<IHandlerWrapper>();
                _handlers.Add(messageClass, subscribers);
            }
            subscribers.Add(new HandlerWrapper<T>(handler, subscriberId));
        }

        return this;
    }
}