namespace DimenshipCommon.Data;

public sealed class GameStateDto
{
    public string PlayerId { get; set; } = string.Empty;
    public string StateId { get; set; } = string.Empty;
    public DateTimeOffset SavedAt { get; set; }
    public string Version { get; set; } = "1.0";
}
