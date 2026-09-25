using System.Linq;
using UnityEngine;

namespace Features.Dish
{
    public class OrderFactory
    {
        private readonly DishData[] _dishes;
        private readonly int _maxOrderSize;

        public OrderFactory(DishData[] dishes, int maxOrderSize)
        {
            _dishes = dishes;
            _maxOrderSize = maxOrderSize;
        }

        public Order Create()
        {
            if (_dishes.Length == 0)
            {
                return null;
            }

            int dishCount = UnityEngine.Random.Range(1, _maxOrderSize + 1);
            DishData[] orderDishes = new DishData[dishCount];
            for (int i = 0; i < dishCount; i++)
            {
                orderDishes[i] = CreateDish();
            }

            return new Order(orderDishes);
        }

        private DishData CreateDish()
        {
            DishData dish = _dishes[UnityEngine.Random.Range(0, _dishes.Length)];
            if (dish.Toppings.Length == 0)
            {
                return dish;
            }

            DishData randomized = Object.Instantiate(dish);
            randomized.name = dish.name;
            randomized.SetToppings(dish.Toppings
                .OrderBy(_ => UnityEngine.Random.value)
                .Take(UnityEngine.Random.Range(1, dish.Toppings.Length + 1))
                .ToArray());
            return randomized;
        }
    }
}
