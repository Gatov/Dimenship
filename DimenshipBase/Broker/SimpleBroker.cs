using System;
using System.Collections.Generic;
using System.Linq;

namespace DimenshipBase.Broker;

public interface IMessage
{
}

public record BrokerError(string SubscriberId, string MessageType, Exception Exception);

public interface IBroker
{
    event Action<BrokerError>? Error;
    void Publish(IMessage message);
    IBroker Subscribe(Action<IMessage> handler, string messageClass, string subscriberId);
    IBroker Subscribe<T>(Action<T> handler, string subscriberId) where T : IMessage;
}

public interface IHandlerWrapper
{
    string SubscriberId { get; }
    void Handle(IMessage m);
}

public class HandlerWrapper<T> : IHandlerWrapper where T : IMessage
{
    private readonly Action<T> _handler;
    private readonly Action<BrokerError>? _onError;

    public string SubscriberId { get; }

    public HandlerWrapper(Action<T> handler, string subscriberId, Action<BrokerError>? onError = null)
    {
        _handler = handler;
        SubscriberId = subscriberId;
        _onError = onError;
    }

    public void Handle(IMessage m)
    {
        try
        {
            _handler.Invoke((T)m);
        }
        catch (Exception ex)
        {
            _onError?.Invoke(new BrokerError(SubscriberId, typeof(T).FullName!, ex));
        }
    }
}

public class SimpleBroker : IBroker
{
    private readonly object _syncRoot = new object();
    public readonly Dictionary<string, List<IHandlerWrapper>> _handlers = new Dictionary<string, List<IHandlerWrapper>>();

    public event Action<BrokerError>? Error;

    public void Publish(IMessage message)
    {
        IHandlerWrapper[] snapshot;
        lock (_syncRoot)
        {
            string messageClass = message.GetType().FullName!;
            if (!_handlers.TryGetValue(messageClass, out var list))
                return;
            snapshot = list.ToArray();
        }
        foreach (var subscriber in snapshot)
            subscriber.Handle(message);
    }

    public IBroker Subscribe(Action<IMessage> handler, string messageClass, string subscriberId)
    {
        lock (_syncRoot)
        {
            if (!_handlers.TryGetValue(messageClass, out var subscribers))
            {
                subscribers = new List<IHandlerWrapper>();
                _handlers.Add(messageClass, subscribers);
            }
            if (subscribers.Any(h => h.SubscriberId == subscriberId))
                throw new InvalidOperationException($"Subscriber '{subscriberId}' is already registered for '{messageClass}'.");
            subscribers.Add(new HandlerWrapper<IMessage>(handler, subscriberId, FireError));
        }
        return this;
    }

    public IBroker Subscribe<T>(Action<T> handler, string subscriberId) where T : IMessage
    {
        lock (_syncRoot)
        {
            string messageClass = typeof(T).FullName!;
            if (!_handlers.TryGetValue(messageClass, out var subscribers))
            {
                subscribers = new List<IHandlerWrapper>();
                _handlers.Add(messageClass, subscribers);
            }
            if (subscribers.Any(h => h.SubscriberId == subscriberId))
                throw new InvalidOperationException($"Subscriber '{subscriberId}' is already registered for '{messageClass}'.");
            subscribers.Add(new HandlerWrapper<T>(handler, subscriberId, FireError));
        }
        return this;
    }

    private void FireError(BrokerError error) => Error?.Invoke(error);
}
