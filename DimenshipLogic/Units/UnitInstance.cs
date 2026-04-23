using DcCoreLib;

namespace DimenshipLogic.Units;

public sealed class UnitInstance
{
    public int Id { get; }
    public string DefinitionId { get; }
    public double Durability { get; private set; }

    public UnitInstance(int id, string definitionId, double durability = 1.0)
    {
        Id = id;
        DefinitionId = Guard.NotNullOrEmpty(definitionId, nameof(definitionId));
        Durability = durability;
    }

    public void ApplyDamage(double amount) =>
        Durability = Math.Max(0, Durability - amount);
}
