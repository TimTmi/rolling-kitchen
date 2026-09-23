using System;
using System.Collections.Generic;
using System.Linq;

namespace Features.Dish
{
    public class Order
    {
        private readonly DishData[] _dishes;

        public IReadOnlyList<DishData> Dishes => _dishes;
        public float ExpectedDuration { get; }

        public Order(params DishData[] dishes)
        {
            _dishes = (DishData[])dishes.Clone();
            ExpectedDuration = _dishes.Sum(dish => dish.ExpectedDuration);
        }
    }
}
