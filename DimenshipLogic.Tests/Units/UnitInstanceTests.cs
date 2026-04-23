using DimenshipLogic.Units;
using NUnit.Framework;

namespace DimenshipLogic.Tests.Units;

[TestFixture]
public class UnitInstanceTests
{
    [Test]
    public void ApplyDamage_ReducesDurability()
    {
        var unit = new UnitInstance(1, "unit.scout", durability: 1.0);
        unit.ApplyDamage(0.3);
        Assert.That(unit.Durability, Is.EqualTo(0.7).Within(0.001));
    }

    [Test]
    public void ApplyDamage_ClampsAtZero()
    {
        var unit = new UnitInstance(1, "unit.scout", durability: 0.1);
        unit.ApplyDamage(999.0);
        Assert.That(unit.Durability, Is.EqualTo(0.0));
    }

    [Test]
    public void Constructor_ThrowsOnEmptyDefinitionId()
    {
        Assert.That(() => new UnitInstance(1, ""), Throws.ArgumentException);
    }
}
