using System;

namespace DimenshipBase
{
    public interface ISystemStateSet
    {
        T GetSubState<T>() where T : ISystemSubState;
        GameTime CurrentTime { get; }
        void AddSubsystem(ISystemSubState subSystem);
    }

    public interface ISystemSubState
    {
        string Id { get; }
        string Name { get; }
    }

    /// <summary>
    /// This should be serializable state, as it will be saved and restored 
    /// </summary>
    [Serializable]
    public class DimenshipEnergyState : ISystemSubState
    {
        public string Id => "EnergyGenerator";
        public string Name => "EnergyGenerator";
        public double AvailableEnergy;
        public double EnergyProductionPerSecond; // efficiency of the energy system
        public GameTime LastTappedTime; // Game time the state represents
        public double Check(GameTime time) => AvailableEnergy + (LastTappedTime - time) * EnergyProductionPerSecond;

        /// <summary>
        /// Request some amount of energy
        /// </summary>
        /// <param name="time"> Time of the request, this will update LastTappedTime in case of success</param>
        /// <param name="requiredAmount"></param>
        /// <returns></returns>
        public bool Tap(GameTime time, double requiredAmount)
        {
            var energyNow = Check(time);
            if (energyNow < requiredAmount) return false;
            LastTappedTime = time;
            AvailableEnergy = energyNow;
            return true;
        }
    }
}