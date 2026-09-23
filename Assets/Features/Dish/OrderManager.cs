using System;
using System.Collections.Generic;
using System.Linq;
using Features.Interaction;
using Features.Pickup;
using UnityEngine;

namespace Features.Dish
{
    public class OrderManager : MonoBehaviour
    {
        [SerializeField] private Tray[] trays = Array.Empty<Tray>();
        [SerializeField] private DishData[] dishes = Array.Empty<DishData>();
        [SerializeField] private float minFreeTime = 5f;
        [SerializeField] private float maxFreeTime = 10f;
        [SerializeField] private int maxOrderSize = 1;

        private Order[] _orders;
        private float[] _spawnTimers;

        public event Action<int, Order> Served;

        public int SlotCount => trays.Length;

        public Order GetOrder(int slotIndex)
        {
            EnsureInitialized();
            return _orders[slotIndex];
        }

        public int FindFreeSlot()
        {
            EnsureInitialized();
            for (int i = 0; i < _orders.Length; i++)
            {
                if (_orders[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool TryPlaceOrder(int slotIndex, Order order)
        {
            EnsureInitialized();
            if (_orders[slotIndex] != null)
            {
                return false;
            }

            _orders[slotIndex] = order;
            return true;
        }

        public void ClearOrder(int slotIndex)
        {
            EnsureInitialized();
            _orders[slotIndex] = null;
        }

        public bool IsDishComplete(int slotIndex, int dishIndex)
        {
            EnsureInitialized();
            Order order = _orders[slotIndex];
            if (order == null || dishIndex < 0 || dishIndex >= order.Dishes.Count)
            {
                return false;
            }

            return MatchesAnyTrayItem(trays[slotIndex], order.Dishes[dishIndex]);
        }

        public bool IsIngredientComplete(int slotIndex, int dishIndex, int ingredientIndex)
        {
            EnsureInitialized();
            Order order = _orders[slotIndex];
            if (order == null || dishIndex < 0 || dishIndex >= order.Dishes.Count)
            {
                return false;
            }

            List<Pickable> required = order.Dishes[dishIndex].RequiredPickables.ToList();
            if (ingredientIndex < 0 || ingredientIndex >= required.Count)
            {
                return false;
            }

            PickableData data = required[ingredientIndex].Data;
            return CountOnTray(trays[slotIndex], data) >= required.Count(r => r.Data == data);
        }

        public bool TryServe(int slotIndex)
        {
            EnsureInitialized();
            Order order = _orders[slotIndex];
            if (order == null)
            {
                return false;
            }

            foreach (DishData dish in order.Dishes)
            {
                if (!MatchesAnyTrayItem(trays[slotIndex], dish))
                {
                    return false;
                }
            }

            _orders[slotIndex] = null;
            Served?.Invoke(slotIndex, order);
            return true;
        }

        private void Update()
        {
            EnsureInitialized();
            for (int i = 0; i < _orders.Length; i++)
            {
                if (_orders[i] != null)
                {
                    continue;
                }

                _spawnTimers[i] -= Time.deltaTime;
                if (_spawnTimers[i] > 0f)
                {
                    continue;
                }

                Order order = GenerateRandomOrder();
                if (order != null)
                {
                    TryPlaceOrder(i, order);
                }

                _spawnTimers[i] = RandomSpawnDelay();
            }
        }

        private Order GenerateRandomOrder()
        {
            if (dishes.Length == 0)
            {
                return null;
            }

            int dishCount = UnityEngine.Random.Range(1, maxOrderSize + 1);
            DishData[] orderDishes = new DishData[dishCount];
            for (int i = 0; i < dishCount; i++)
            {
                orderDishes[i] = GenerateRandomDish();
            }

            return new Order(orderDishes);
        }

        private DishData GenerateRandomDish()
        {
            if (dishes.Length == 0)
            {
                return null;
            }

            DishData dish = dishes[UnityEngine.Random.Range(0, dishes.Length)];
            if (dish.Toppings.Length == 0)
            {
                return dish;
            }

            DishData randomized = Instantiate(dish);
            randomized.name = dish.name;
            randomized.SetToppings(dish.Toppings
                .OrderBy(_ => UnityEngine.Random.value)
                .Take(UnityEngine.Random.Range(1, dish.Toppings.Length + 1))
                .ToArray());
            return randomized;
        }

        private float RandomSpawnDelay()
        {
            return UnityEngine.Random.Range(minFreeTime, maxFreeTime);
        }

        private void EnsureInitialized()
        {
            if (_orders == null || _orders.Length != trays.Length)
            {
                _orders = new Order[trays.Length];
                _spawnTimers = new float[trays.Length];
                for (int i = 0; i < _spawnTimers.Length; i++)
                {
                    _spawnTimers[i] = RandomSpawnDelay();
                }
            }
        }

        private static bool MatchesAnyTrayItem(Tray tray, DishData dish)
        {
            foreach (Pickable item in tray.Contents)
            {
                if (MatchesDish(item, dish))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<Pickable> Expand(Pickable item)
        {
            return item.Stack != null ? item.Stack.Contents : new[] { item };
        }

        private static int CountOnTray(Tray tray, PickableData data)
        {
            int count = 0;
            foreach (Pickable item in tray.Contents)
            {
                foreach (Pickable content in Expand(item))
                {
                    if (content.Data == data)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static bool MatchesDish(Pickable item, DishData dish)
        {
            List<PickableData> present = new();
            foreach (Pickable content in Expand(item))
            {
                present.Add(content.Data);
            }

            List<PickableData> required = new();
            foreach (Pickable pickable in dish.RequiredPickables)
            {
                required.Add(pickable.Data);
            }

            if (present.Count != required.Count)
            {
                return false;
            }

            foreach (PickableData data in required)
            {
                if (!present.Remove(data))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
