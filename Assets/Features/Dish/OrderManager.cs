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

            DishData dish = order.Dishes[dishIndex];

            int earlierDuplicates = 0;
            for (int i = 0; i < dishIndex; i++)
            {
                if (IsSameDish(order.Dishes[i], dish))
                {
                    earlierDuplicates++;
                }
            }

            return CountMatches(trays[slotIndex], dish) > earlierDuplicates;
        }

        public bool TryServe(int slotIndex)
        {
            EnsureInitialized();
            Order order = _orders[slotIndex];
            if (order == null)
            {
                return false;
            }

            for (int i = 0; i < order.Dishes.Count; i++)
            {
                if (!IsDishComplete(slotIndex, i))
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

        private static IEnumerable<Pickable> Expand(Pickable item)
        {
            return item.Stack != null ? item.Stack.Contents : new[] { item };
        }

        private static int CountMatches(Tray tray, DishData dish)
        {
            int count = 0;
            foreach (Pickable item in tray.Contents)
            {
                if (MatchesDish(item, dish))
                {
                    count++;
                }
            }

            return count;
        }

        private static bool IsSameDish(DishData a, DishData b)
        {
            List<PickableData> required = new();
            foreach (Pickable pickable in a.RequiredPickables)
            {
                required.Add(pickable.Data);
            }

            foreach (Pickable pickable in b.RequiredPickables)
            {
                if (!required.Remove(pickable.Data))
                {
                    return false;
                }
            }

            return required.Count == 0;
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
