using DimenshipCommon.Data;

namespace DimenshipCommon.Interfaces;

public interface IPlayerRepository
{
    Task<PlayerDto?> GetByIdAsync(string playerId, CancellationToken ct = default);
    Task UpsertAsync(PlayerDto player, CancellationToken ct = default);
}
