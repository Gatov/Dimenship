using NUnit.Framework;

namespace DcCoreLib.Tests;

[TestFixture]
public class BrokerTests
{
    private record TestMessage(string Value) : IMessage;
    private record OtherMessage(int Number) : IMessage;

    [Test]
    public void Subscribe_ReceivesPublishedMessage()
    {
        var broker = new Broker();
        TestMessage? received = null;
        broker.Subscribe<TestMessage>(m => received = m, "sub1");

        broker.Publish(new TestMessage("hello"));

        Assert.That(received, Is.Not.Null);
        Assert.That(received!.Value, Is.EqualTo("hello"));
    }

    [Test]
    public void Publish_NoSubscribers_DoesNotThrow()
    {
        var broker = new Broker();
        Assert.That(() => broker.Publish(new TestMessage("x")), Throws.Nothing);
    }

    [Test]
    public void Unsubscribe_BySubscriberId_StopsReceivingMessages()
    {
        var broker = new Broker();
        var count = 0;
        broker.Subscribe<TestMessage>(_ => count++, "sub1");

        broker.Publish(new TestMessage("first"));
        broker.Unsubscribe("sub1");
        broker.Publish(new TestMessage("second"));

        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public void DisposeSubscription_StopsReceivingMessages()
    {
        var broker = new Broker();
        var count = 0;
        var subscription = broker.Subscribe<TestMessage>(_ => count++, "sub1");

        broker.Publish(new TestMessage("first"));
        subscription.Dispose();
        broker.Publish(new TestMessage("second"));

        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public void MultipleSubscribers_AllReceiveMessage()
    {
        var broker = new Broker();
        var received = new List<string>();
        broker.Subscribe<TestMessage>(m => received.Add("A:" + m.Value), "subA");
        broker.Subscribe<TestMessage>(m => received.Add("B:" + m.Value), "subB");

        broker.Publish(new TestMessage("ping"));

        Assert.That(received, Is.EquivalentTo(new[] { "A:ping", "B:ping" }));
    }

    [Test]
    public void Subscribe_DifferentTypes_OnlyCorrectTypeReceives()
    {
        var broker = new Broker();
        var testCount = 0;
        var otherCount = 0;
        broker.Subscribe<TestMessage>(_ => testCount++, "sub1");
        broker.Subscribe<OtherMessage>(_ => otherCount++, "sub2");

        broker.Publish(new TestMessage("x"));

        Assert.That(testCount, Is.EqualTo(1));
        Assert.That(otherCount, Is.EqualTo(0));
    }

    [Test]
    public void HandlerException_DoesNotBreakOtherHandlers()
    {
        var broker = new Broker();
        var secondCalled = false;
        broker.Subscribe<TestMessage>(_ => throw new InvalidOperationException("boom"), "bad");
        broker.Subscribe<TestMessage>(_ => secondCalled = true, "good");

        Assert.That(() => broker.Publish(new TestMessage("x")), Throws.Nothing);
        Assert.That(secondCalled, Is.True);
    }

    [Test]
    public void HandlerException_FiresErrorEvent()
    {
        var broker = new Broker();
        BrokerError? capturedError = null;
        broker.Error += err => capturedError = err;
        broker.Subscribe<TestMessage>(_ => throw new InvalidOperationException("boom"), "bad");

        broker.Publish(new TestMessage("x"));

        Assert.That(capturedError, Is.Not.Null);
        Assert.That(capturedError!.SubscriberId, Is.EqualTo("bad"));
        Assert.That(capturedError.Exception.Message, Is.EqualTo("boom"));
        Assert.That(capturedError.MessageType, Does.Contain("TestMessage"));
    }

    [Test]
    public void Subscribe_DuplicateSubscriberIdForSameType_Throws()
    {
        var broker = new Broker();
        broker.Subscribe<TestMessage>(_ => { }, "sub1");

        Assert.That(
            () => broker.Subscribe<TestMessage>(_ => { }, "sub1"),
            Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Subscribe_SameSubscriberIdForDifferentTypes_IsAllowed()
    {
        var broker = new Broker();

        Assert.That(() =>
        {
            broker.Subscribe<TestMessage>(_ => { }, "sub1");
            broker.Subscribe<OtherMessage>(_ => { }, "sub1");
        }, Throws.Nothing);
    }

    [Test]
    public void Subscribe_AfterUnsubscribe_AllowsReuseOfSubscriberId()
    {
        var broker = new Broker();
        broker.Subscribe<TestMessage>(_ => { }, "sub1");
        broker.Unsubscribe("sub1");

        Assert.That(() => broker.Subscribe<TestMessage>(_ => { }, "sub1"), Throws.Nothing);
    }
}
