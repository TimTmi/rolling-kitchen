using System;
using System.Collections.Generic;
using System.Linq;
using Features.Ingredient;
using Features.Pickup;
using UnityEngine;

namespace Features.Dish
{
    [CreateAssetMenu(fileName = "DishData", menuName = "Scriptable Objects/DishData")]
    public class DishData : ScriptableObject
    {
        [SerializeField] private Pickup.Ingredient baseIngredient;
        [SerializeField] private Pickup.Ingredient topIngredient;
        [SerializeField] private Pickup.Ingredient[] toppings = Array.Empty<Pickup.Ingredient>();

        public Pickup.Ingredient BaseIngredient => baseIngredient;
        public Pickup.Ingredient TopIngredient => topIngredient;
        public Pickup.Ingredient[] Toppings => toppings;

        public IEnumerable<Pickup.Ingredient> RequiredIngredients
        {
            get
            {
                if (baseIngredient != null)
                {
                    yield return baseIngredient;
                }

                if (topIngredient != null)
                {
                    yield return topIngredient;
                }

                foreach (Pickup.Ingredient topping in toppings)
                {
                    if (topping != null)
                    {
                        yield return topping;
                    }
                }
            }
        }

        public float ExpectedDuration =>
            IngredientProcessGraph.ExpectedDuration(
                RequiredIngredients.Select(ingredient => ingredient.IngredientData).ToArray());
    }
}
