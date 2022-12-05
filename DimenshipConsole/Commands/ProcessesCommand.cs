using DimenshipBase;
using DimenshipBase.SubSystems;

namespace DimenshipConsole.Commands;

public class ProcessesCommand : ICommand
{
    public string Command => "p";
    public string HelpLine => "p : list all processes that are currently running";
    public bool Execute(ISystemStateSet system, string[] args)
    {
        var curTime = system.CurrentTime;
        var ps = system.GetSubState<ProcessSubSystem>();
        foreach (var proc in ps.List)
        {
            Console.WriteLine($"{proc.UniqueId,12} | {proc.Complete,5} | {TimeSpan.FromSeconds(proc.FinishBy - curTime)}");   
        }

        return true;

    }
}