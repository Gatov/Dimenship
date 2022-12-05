using System.Linq;
using DimenshipBase;
using DimenshipBase.SubSystems;
using NUnit.Framework;

namespace DimenshipBaseTests;

[TestFixture]
public class BotAssemblyTests
{
    private ISystemStateSet CreateSystemWithBot()
    {
        BotsSubSystem bs = new BotsSubSystem();
        var system = SystemTestHelper.GenerateSystem();
        system.AddSubsystem(bs);
        var sd = system.GetSubState<StaticDataSubSystem>();
        var power = sd.GetAllItemClasses().First(x => x.Tags.Contains("power block"));
        var chassis = sd.GetAllItemClasses().First(x => x.Tags.Contains("chassis"));
        var sensor = sd.GetAllItemClasses().First(x => x.Tags.Contains("sensor"));
        var shield = sd.GetAllItemClasses().First(x => x.Tags.Contains("shield"));
        var weapon = sd.GetAllItemClasses().First(x => x.Tags.Contains("weapon"));
        
        Bot newAssaultBot = new Bot() {BotName = "NewAssaultBot1"};
        newAssaultBot.Frame = power.Id;
        newAssaultBot.Chassis = chassis.Id;
        newAssaultBot.Sensor = sensor.Id;
        newAssaultBot.Shield = shield.Id;
        newAssaultBot.Tool = weapon.Id;
        bs.Enlist(newAssaultBot);
        return system;
    }
}