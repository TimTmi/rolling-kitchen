using System;
using System.Collections.Generic;
using System.Linq;
using Features.Ingredients;
using Features.Pickup;
using UnityEngine;

namespace Features.Dish
{
    [CreateAssetMenu(fileName = "DishData", menuName = "Scriptable Objects/DishData")]
    public class DishData : ScriptableObject
    {
        [SerializeField] private Pickable box;
        [SerializeField] private Ingredient baseIngredient;
        [SerializeField] private Ingredient topIngredient;
        [SerializeField] private Ingredient[] toppings = Array.Empty<Ingredient>();

        public Pickable Box => box;
        public Ingredient BaseIngredient => baseIngredient;
        public Ingredient TopIngredient => topIngredient;
        public Ingredient[] Toppings => toppings;

        public void SetToppings(Ingredient[] value)
        {
            toppings = value;
        }

        public IEnumerable<Pickable> RequiredPickables
        {
            get
            {
                if (box != null)
                {
                    yield return box;
                }

                foreach (Ingredient ingredient in RequiredIngredients)
                {
                    yield return ingredient;
                }
            }
        }

        public IEnumerable<Ingredient> RequiredIngredients
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

                foreach (Ingredient topping in toppings)
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
