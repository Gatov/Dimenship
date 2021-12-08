using System;

namespace DimenshipBase
{
    public struct GameTime
    {
        private static readonly DateTime GameUtcEpoch = new DateTime(2020,1,1,0,0,0,DateTimeKind.Utc);
        private static readonly DateTime GameLocalEpoch = GameUtcEpoch.ToLocalTime();
        public int TimeInTicks;
        public DateTime AsUtcTime()
        {
            return GameUtcEpoch.AddSeconds(TimeInTicks);
        }
        public DateTime AsLocalTime()
        {
            return GameLocalEpoch.AddSeconds(TimeInTicks);
        }

        public GameTime AddTicks(int ticks)
        {
            return new GameTime() {TimeInTicks = TimeInTicks + ticks};
        }

        public static int operator -(GameTime a, GameTime b)
        {
            return a.TimeInTicks - b.TimeInTicks;
        }
    }
}