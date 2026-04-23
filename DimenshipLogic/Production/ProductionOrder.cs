namespace DimenshipLogic.Production;

public sealed class ProductionOrder
{
    public int OrderId { get; init; }
    public string RecipeId { get; init; } = string.Empty;
    public int Quantity { get; init; } = 1;
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset EstimatedCompletion { get; init; }
    public bool IsComplete { get; private set; }

    public void MarkComplete() => IsComplete = true;
}
