namespace DcCoreLib;

public interface IMessage { }

public record BrokerError(string SubscriberId, string MessageType, Exception Exception);

public interface IBroker
{
    event Action<BrokerError>? Error;
    void Publish(IMessage message);
    IDisposable Subscribe<T>(Action<T> handler, string subscriberId) where T : IMessage;
    void Unsubscribe(string subscriberId);
}

internal interface IHandlerWrapper
{
    string SubscriberId { get; }
    void Handle(IMessage message);
}

internal sealed class HandlerWrapper<T> : IHandlerWrapper where T : IMessage
{
    private readonly Action<T> _handler;
    private readonly Action<BrokerError>? _onError;

    public string SubscriberId { get; }

    public HandlerWrapper(Action<T> handler, string subscriberId, Action<BrokerError>? onError)
    {
        _handler = handler;
        SubscriberId = subscriberId;
        _onError = onError;
    }

    public void Handle(IMessage message)
    {
        try
        {
            _handler((T)message);
        }
        catch (Exception ex)
        {
            _onError?.Invoke(new BrokerError(SubscriberId, typeof(T).FullName!, ex));
        }
    }
}

internal sealed class Subscription : IDisposable
{
    private readonly IBroker _broker;
    private readonly string _subscriberId;
    private bool _disposed;

    public Subscription(IBroker broker, string subscriberId)
    {
        _broker = broker;
        _subscriberId = subscriberId;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _broker.Unsubscribe(_subscriberId);
    }
}

public sealed class Broker : IBroker
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<string, List<IHandlerWrapper>> _handlers = new();

    public event Action<BrokerError>? Error;

    public void Publish(IMessage message)
    {
        IHandlerWrapper[] snapshot;
        lock (_syncRoot)
        {
            if (!_handlers.TryGetValue(message.GetType().FullName!, out var list))
                return;
            snapshot = list.ToArray();
        }
        foreach (var h in snapshot)
            h.Handle(message);
    }

    public IDisposable Subscribe<T>(Action<T> handler, string subscriberId) where T : IMessage
    {
        lock (_syncRoot)
        {
            var key = typeof(T).FullName!;
            if (!_handlers.TryGetValue(key, out var list))
            {
                list = new List<IHandlerWrapper>();
                _handlers[key] = list;
            }
            if (list.Any(h => h.SubscriberId == subscriberId))
                throw new InvalidOperationException($"Subscriber '{subscriberId}' is already registered for '{key}'.");
            list.Add(new HandlerWrapper<T>(handler, subscriberId, FireError));
        }
        return new Subscription(this, subscriberId);
    }

    public void Unsubscribe(string subscriberId)
    {
        lock (_syncRoot)
        {
            foreach (var list in _handlers.Values)
                list.RemoveAll(h => h.SubscriberId == subscriberId);
        }
    }

    private void FireError(BrokerError error) => Error?.Invoke(error);
}
