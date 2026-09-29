using UnityEngine;

namespace Features.Service
{
    public readonly struct EndlessDifficulty
    {
        private const int MaxOrderSizeLimit = 3;
        private const float MinFreeTimeLimit = 0f;
        private const float MinWaitTimeMultiplierLimit = 1f;
        private const int MinRepGainLimit = 0;

        private EndlessDifficulty(int maxOrderSize, float minFreeTime, float maxFreeTime, float waitTimeMultiplier, int repGain)
        {
            MaxOrderSize = maxOrderSize;
            MinFreeTime = minFreeTime;
            MaxFreeTime = maxFreeTime;
            WaitTimeMultiplier = waitTimeMultiplier;
            RepGain = repGain;
        }

        public int MaxOrderSize { get; }
        public float MinFreeTime { get; }
        public float MaxFreeTime { get; }
        public float WaitTimeMultiplier { get; }
        public int RepGain { get; }

        public static EndlessDifficulty ForServedOrders(LevelData start, int rampOrders, int servedOrders)
        {
            float t = Mathf.Clamp01(servedOrders / (float)Mathf.Max(1, rampOrders));
            return new EndlessDifficulty(
                Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(start.MaxOrderSize, MaxOrderSizeLimit, t)), int.MinValue, MaxOrderSizeLimit),
                Mathf.Clamp(Mathf.Lerp(start.MinFreeTime, MinFreeTimeLimit, t), MinFreeTimeLimit, float.MaxValue),
                Mathf.Clamp(Mathf.Lerp(start.MaxFreeTime, MinFreeTimeLimit, t), MinFreeTimeLimit, float.MaxValue),
                Mathf.Clamp(Mathf.Lerp(start.WaitTimeMultiplier, MinWaitTimeMultiplierLimit, t), MinWaitTimeMultiplierLimit, float.MaxValue),
                Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(start.RepGain, MinRepGainLimit, t)), MinRepGainLimit, int.MaxValue));
        }
    }
}
