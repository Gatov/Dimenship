namespace DimenshipLogic.Game;

public sealed class GameContext : IGameContext
{
    private readonly Dictionary<Type, object> _services = new();

    public DateTimeOffset CurrentTime { get; private set; } = DateTimeOffset.UtcNow;

    public void Register<T>(T service) where T : class
        => _services[typeof(T)] = service;

    public T GetService<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out var svc))
            return (T)svc;
        throw new InvalidOperationException($"Service {typeof(T).Name} is not registered.");
    }

    public void AdvanceTime(DateTimeOffset newTime) => CurrentTime = newTime;
}
