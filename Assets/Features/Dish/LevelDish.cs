using System;
using Features.Pickup;
using UnityEngine;

namespace Features.Dish
{
    [Serializable]
    public class LevelDish
    {
        [SerializeField] private DishData dish;
        [SerializeField] private Ingredient[] allowedToppings = Array.Empty<Ingredient>();

        public DishData Dish => dish;

        public Ingredient[] AllowedToppings => allowedToppings;
    }
}
