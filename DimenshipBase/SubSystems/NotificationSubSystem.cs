using System;

namespace DimenshipBase.SubSystems;

public class NotificationSubSystem : ISystemSubState
{
    public string Id => "Notification";
    public string Name =>"Notification";
    public event Action<string> InfoAction;
    public event Action<string> PushNotificationAction;

    // stub
    public void LogLine(string line)
    {
        InfoAction?.Invoke(line);
        //Console.WriteLine(line);
    }
    public void Notify(string line)
    {
        PushNotificationAction?.Invoke(line);
        //Console.WriteLine("! " + line);
    }
}