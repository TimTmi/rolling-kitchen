using System;
using Features.Dish;
using UnityEngine;

namespace Features.Customer
{
    public class CustomerScheduler : MonoBehaviour
    {
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private CustomerController customerPrefab;
        [SerializeField] private DishData[] dishes = Array.Empty<DishData>();
        [SerializeField] private int maxOrderSize = 1;
        [SerializeField] private float minFreeTime = 5f;
        [SerializeField] private float maxFreeTime = 10f;

        private OrderFactory _orderFactory;
        private float[] _spawnTimers;
        private CustomerController[] _customers;
        private SlotState[] _slotStates;

        private enum SlotState
        {
            Free,
            Incoming,
            Returning
        }

        private void Awake()
        {
            _orderFactory = new OrderFactory(dishes, maxOrderSize);
        }

        private void OnEnable()
        {
            orderManager.Served += OnOrderServed;
        }

        private void OnDisable()
        {
            orderManager.Served -= OnOrderServed;
        }

        private void Start()
        {
            SpawnCustomers();
        }

        private void Update()
        {
            EnsureSpawnTimersInitialized();
            for (int i = 0; i < _spawnTimers.Length; i++)
            {
                if (orderManager.GetOrder(i) != null || _slotStates[i] != SlotState.Free)
                {
                    continue;
                }

                _spawnTimers[i] -= Time.deltaTime;
                if (_spawnTimers[i] > 0f)
                {
                    continue;
                }

                SendCustomerToCounter(i);
            }
        }

        public void SetFreeTimeRange(float min, float max)
        {
            minFreeTime = min;
            maxFreeTime = max;
        }

        private void SendCustomerToCounter(int slotIndex)
        {
            _slotStates[slotIndex] = SlotState.Incoming;
            OrderSlot slot = orderManager.GetSlot(slotIndex);
            CustomerController customer = _customers[slotIndex];
            customer.transform.position = slot.PathStart.position;
            customer.WalkTo(slot.PathEnd);
        }

        private void OnCustomerArrived(int slotIndex)
        {
            if (_slotStates[slotIndex] == SlotState.Incoming)
            {
                Order order = _orderFactory.Create();
                if (order != null)
                {
                    orderManager.TryPlaceOrder(slotIndex, order);
                }
            }

            _slotStates[slotIndex] = SlotState.Free;
            _spawnTimers[slotIndex] = RandomSpawnDelay();
        }

        private void OnOrderServed(int slotIndex, Order order)
        {
            _slotStates[slotIndex] = SlotState.Returning;
            OrderSlot slot = orderManager.GetSlot(slotIndex);
            _customers[slotIndex].WalkTo(slot.PathStart);
        }

        private void EnsureSpawnTimersInitialized()
        {
            if (_spawnTimers == null || _spawnTimers.Length != orderManager.SlotCount)
            {
                _spawnTimers = new float[orderManager.SlotCount];
                for (int i = 0; i < _spawnTimers.Length; i++)
                {
                    _spawnTimers[i] = RandomSpawnDelay();
                }
            }
        }

        private void SpawnCustomers()
        {
            _customers = new CustomerController[orderManager.SlotCount];
            _slotStates = new SlotState[orderManager.SlotCount];
            for (int i = 0; i < _customers.Length; i++)
            {
                OrderSlot slot = orderManager.GetSlot(i);
                CustomerController customer = Instantiate(customerPrefab, transform);
                customer.Init(orderManager, i);
                customer.transform.position = slot.PathStart.position;
                int slotIndex = i;
                customer.Arrived += () => OnCustomerArrived(slotIndex);
                _customers[i] = customer;
            }
        }

        private float RandomSpawnDelay()
        {
            return UnityEngine.Random.Range(minFreeTime, maxFreeTime);
        }
    }
}
