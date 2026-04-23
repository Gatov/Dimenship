using DimenshipLogic.Game;
using NUnit.Framework;

namespace DimenshipLogic.Tests.Game;

[TestFixture]
public class GameContextTests
{
    [Test]
    public void GetService_ReturnsRegisteredService()
    {
        var ctx = new GameContext();
        var service = new FakeService();
        ctx.Register(service);

        var retrieved = ctx.GetService<FakeService>();

        Assert.That(retrieved, Is.SameAs(service));
    }

    [Test]
    public void GetService_ThrowsWhenNotRegistered()
    {
        var ctx = new GameContext();
        Assert.That(() => ctx.GetService<FakeService>(), Throws.InvalidOperationException);
    }

    private sealed class FakeService { }
}
