using DimenshipCommon.Data;

namespace DimenshipCommon.Interfaces;

public interface IGameStateRepository
{
    Task<GameStateDto?> LoadAsync(string playerId, CancellationToken ct = default);
    Task SaveAsync(GameStateDto state, CancellationToken ct = default);
    Task DeleteAsync(string playerId, CancellationToken ct = default);
}
