using System;
using System.Collections.Generic;
using System.Linq;

namespace Features.Dish
{
    public class Order
    {
        public IReadOnlyList<DishData> Dishes { get; }
        public float ExpectedDuration { get; }

        public Order(params DishData[] dishes)
        {
            Dishes = (DishData[])dishes.Clone();
            ExpectedDuration = Dishes.Sum(dish => dish.ExpectedDuration);
        }
    }
}
