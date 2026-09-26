using System;
using System.Collections.Generic;
using System.Linq;
using Features.Dish;
using Features.Ingredients;
using Features.Pickup;
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

        public IngredientData[] GetIngredients()
        {
            HashSet<IngredientData> ingredients = new HashSet<IngredientData>();
            foreach (LevelDish entry in dishes)
            {
                if (entry?.Dish == null)
                {
                    continue;
                }

                CollectWithSources(entry.Dish.BaseIngredient, ingredients);
                CollectWithSources(entry.Dish.TopIngredient, ingredients);
                foreach (Ingredient topping in entry.GetToppingPool())
                {
                    CollectWithSources(topping, ingredients);
                }
            }

            return ingredients.ToArray();
        }

        private static void CollectWithSources(Ingredient ingredient, HashSet<IngredientData> result)
        {
            CollectWithSources(ingredient == null ? null : ingredient.IngredientData, result);
        }

        private static void CollectWithSources(IngredientData data, HashSet<IngredientData> result)
        {
            if (data == null || !result.Add(data))
            {
                return;
            }

            foreach (IngredientProcessGraph.Producer producer in IngredientProcessGraph.GetProducers(data))
            {
                CollectWithSources(producer.Source, result);
            }
        }
    }
}
