using System.Linq;
using Features.Pickup;
using UnityEngine;

namespace Features.Dish
{
    public class OrderFactory
    {
        private readonly LevelDish[] _dishes;
        private readonly int _maxOrderSize;

        public OrderFactory(LevelDish[] dishes, int maxOrderSize)
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
            LevelDish entry = _dishes[UnityEngine.Random.Range(0, _dishes.Length)];
            DishData dish = entry.Dish;
            Ingredient[] toppingPool = entry.GetToppingPool();
            if (toppingPool.Length == 0)
            {
                return dish;
            }

            DishData randomized = Object.Instantiate(dish);
            randomized.name = dish.name;
            randomized.SetToppings(toppingPool
                .OrderBy(_ => UnityEngine.Random.value)
                .Take(UnityEngine.Random.Range(1, toppingPool.Length + 1))
                .ToArray());
            return randomized;
        }
    }
}
