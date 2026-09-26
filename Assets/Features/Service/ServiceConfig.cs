using System;
using Features.Dish;
using Features.Pickup;
using UnityEngine;

namespace Features.Service
{
    public enum GameMode
    {
        Campaign,
        Endless
    }

    [CreateAssetMenu(fileName = "ServiceConfig", menuName = "Scriptable Objects/ServiceConfig")]
    public class ServiceConfig : ScriptableObject
    {
        [SerializeField] private GameMode gameMode = GameMode.Campaign;
        [SerializeField] private int level = 1;
        [SerializeField] private int maxOrderSize = 1;
        [SerializeField] private int orderCount = 10;
        [SerializeField] private float minFreeTime = 5f;
        [SerializeField] private float maxFreeTime = 10f;
        [SerializeField] private float waitTimeMultiplier = 1f;
        [SerializeField] private int maxRep = 100;
        [SerializeField] private int repLoss = -5;
        [SerializeField] private int repGain = 2;
        [SerializeField] private DishData[] availableDishes = Array.Empty<DishData>();
        [SerializeField] private Ingredient[] availableToppings = Array.Empty<Ingredient>();

        public GameMode GameMode => gameMode;

        public int Level => level;

        public int MaxOrderSize => maxOrderSize;

        public int OrderCount => orderCount;

        public float MinFreeTime => minFreeTime;

        public float MaxFreeTime => maxFreeTime;

        public float WaitTimeMultiplier => waitTimeMultiplier;

        public int MaxRep => maxRep;

        public int RepLoss => repLoss;

        public int RepGain => repGain;

        public DishData[] AvailableDishes => availableDishes;

        public Ingredient[] AvailableToppings => availableToppings;
    }
}
