using DimenshipBase;

namespace DimenshipConsole.Commands;

public interface ICommand
{
    string Command { get; }
    string HelpLine { get; }

    bool Execute(ISystemStateSet system, string [] args);
}