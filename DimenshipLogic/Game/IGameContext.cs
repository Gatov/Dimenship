namespace DimenshipLogic.Game;

public interface IGameContext
{
    T GetService<T>() where T : class;
    DateTimeOffset CurrentTime { get; }
}
