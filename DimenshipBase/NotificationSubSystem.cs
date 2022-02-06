using System;

namespace DimenshipBase;

public class NotificationSubSystem : ISystemSubState
{
    public string Id => "Notification";
    public string Name =>"Notification";

    // stub
    public void LogLine(string line)
    {
        Console.WriteLine(line);
    }
    public void Notify(string line)
    {
        Console.WriteLine("! " + line);
    }
}