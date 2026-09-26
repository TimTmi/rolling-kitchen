using System;
using Features.Dish;
using UnityEngine;

namespace Features.Service
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private int maxOrderSize = 1;
        [SerializeField] private int orderCount = 10;
        [SerializeField] private float minFreeTime = 5f;
        [SerializeField] private float maxFreeTime = 10f;
        [SerializeField] private float waitTimeMultiplier = 1f;
        [SerializeField] private int maxRep = 100;
        [SerializeField] private int repLoss = -5;
        [SerializeField] private int repGain = 2;
        [SerializeField] private LevelDish[] dishes = Array.Empty<LevelDish>();

        public int MaxOrderSize => maxOrderSize;

        public int OrderCount => orderCount;

        public float MinFreeTime => minFreeTime;

        public float MaxFreeTime => maxFreeTime;

        public float WaitTimeMultiplier => waitTimeMultiplier;

        public int MaxRep => maxRep;

        public int RepLoss => repLoss;

        public int RepGain => repGain;

        public LevelDish[] Dishes => dishes;
    }
}
