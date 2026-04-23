namespace DcCoreLib;

public sealed class IdGenerator
{
    private int _current;

    public int Next() => Interlocked.Increment(ref _current);
    public void Reset() => Interlocked.Exchange(ref _current, 0);
}
