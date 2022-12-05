using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DimenshipBase.SubSystems;

[DataContract(Name = "BotSystem")]
public class BotsSubSystem : ISystemSubState
{
    public string Id => "Bots";
    public string Name => "Bots";

    private readonly object _syncRoot = new Object();
    private int lastAllocatedBotId = 0;

    [DataMember]
    private List<Bot> BotList{ get; set; }


    public Bot Decommission(int botId)
    {
        lock (_syncRoot)
        {
            int idx = BotList.FindIndex(x => x.BotId == botId);
            if (idx < 0) return null;
            var bot = BotList[idx];
            BotList.RemoveAt(idx);
            return bot;
        }
    }

    public void Enlist(Bot bot)
    {
        lock (_syncRoot)
        {
            bot.BotId = ++lastAllocatedBotId;
            BotList.Add(bot);
        }
    }
    
    public List<(Bot bot, int inProcess)> ListBots()
    {
        throw new NotImplementedException();
    }
}

[DataContract(Name = "Bot")]
public class Bot
{
    // construction
    [DataMember] public int BotId { get; set; }
    [DataMember] public string BotName { get; set; }
    [DataMember] public string Frame { get; set; }
    [DataMember] public string Chassis { get; set; }
    [DataMember] public string Sensor { get; set; }
    [DataMember] public string Tool { get; set; }
    [DataMember] public string Shield { get; set; }

    // current parameters
    [DataMember] public double Durability { get; set; }
    // TODO: More parameters, like AI coefficient, aka experience
}

