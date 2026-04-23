using System.Diagnostics;

namespace DcCoreLib;

public interface IMessage { }

public interface IBroker
{
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

    public string SubscriberId { get; }

    public HandlerWrapper(Action<T> handler, string subscriberId)
    {
        _handler = handler;
        SubscriberId = subscriberId;
    }

    public void Handle(IMessage message)
    {
        try
        {
            _handler((T)message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Handler for {typeof(T).Name} from '{SubscriberId}' threw: {ex.Message}");
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

    public void Publish(IMessage message)
    {
        IHandlerWrapper[] snapshot;
        lock (_syncRoot)
        {
            if (!_handlers.TryGetValue(message.GetType().Name, out var list))
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
            var key = typeof(T).Name;
            if (!_handlers.TryGetValue(key, out var list))
            {
                list = new List<IHandlerWrapper>();
                _handlers[key] = list;
            }
            list.Add(new HandlerWrapper<T>(handler, subscriberId));
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
}
