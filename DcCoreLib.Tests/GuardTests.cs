using DcCoreLib;
using NUnit.Framework;

namespace DcCoreLib.Tests;

[TestFixture]
public class GuardTests
{
    [Test]
    public void NotNull_ThrowsOnNull()
    {
        Assert.That(() => Guard.NotNull<string>(null, "param"), Throws.ArgumentNullException);
    }

    [Test]
    public void NotNull_ReturnsValueWhenNotNull()
    {
        var value = Guard.NotNull("hello", "param");
        Assert.That(value, Is.EqualTo("hello"));
    }

    [Test]
    public void NotNullOrEmpty_ThrowsOnEmpty()
    {
        Assert.That(() => Guard.NotNullOrEmpty("", "param"), Throws.ArgumentException);
    }
}
