using System;

namespace DimenshipBase
{
    public struct GameTime
    {
        private static readonly DateTime GameUtcEpoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime GameLocalEpoch = GameUtcEpoch.ToLocalTime();
        public int TimeInTicks;

        public GameTime(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Local)
                TimeInTicks = (int)(dt - GameLocalEpoch).TotalSeconds;
            else
                TimeInTicks = (int)(dt - GameUtcEpoch).TotalSeconds;
        }

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
            return new GameTime() { TimeInTicks = TimeInTicks + ticks };
        }

        public static int operator -(GameTime a, GameTime b)
        {
            return a.TimeInTicks - b.TimeInTicks;
        }

        public static GameTime operator +(GameTime a, int ticks)
        {
            return new GameTime() { TimeInTicks = a.TimeInTicks + ticks };
        }

        public static implicit operator int(GameTime a) => a.TimeInTicks;
    }
}