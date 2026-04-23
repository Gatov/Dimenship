using DcCoreLib;
using NUnit.Framework;

namespace DcCoreLib.Tests;

[TestFixture]
public class ResultTests
{
    [Test]
    public void Ok_SetsIsSuccessTrue()
    {
        var result = Result<int>.Ok(42);
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(42));
    }

    [Test]
    public void Fail_SetsIsSuccessFalse()
    {
        var result = Result<int>.Fail("something went wrong");
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("something went wrong"));
    }
}
