using System;
using Features.Customer;
using Features.Dish;
using Features.Service;
using UnityEngine;

namespace Features.Reputation
{
    public class ReputationController : MonoBehaviour
    {
        [SerializeField] private CustomerScheduler customerScheduler;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private ServiceConfig serviceConfig;

        private int _rep;
        private int _servedOrders;

        public event Action<int> ReputationChanged;

        public int Rep => _rep;
        public int MaxRep => serviceConfig.ActiveLevel.MaxRep;

        private void Awake()
        {
            _rep = serviceConfig.ActiveLevel.MaxRep;
        }

        private void Start()
        {
            ReputationChanged?.Invoke(_rep);
        }

        private void OnEnable()
        {
            customerScheduler.OrderTimedOut += OnOrderTimedOut;
            orderManager.Served += OnOrderServed;
        }

        private void OnDisable()
        {
            customerScheduler.OrderTimedOut -= OnOrderTimedOut;
            orderManager.Served -= OnOrderServed;
        }

        private void OnOrderTimedOut(int slotIndex, Order order)
        {
            AddRep(serviceConfig.ActiveLevel.RepLoss);
        }

        private void OnOrderServed(int slotIndex, Order order)
        {
            _servedOrders++;
            AddRep(GetRepGain());
        }

        private int GetRepGain()
        {
            if (serviceConfig.GameMode != GameMode.Endless)
            {
                return serviceConfig.ActiveLevel.RepGain;
            }

            return EndlessDifficulty.ForServedOrders(serviceConfig.Levels[0], serviceConfig.EndlessRampOrders, _servedOrders).RepGain;
        }

        private void AddRep(int delta)
        {
            _rep = Mathf.Clamp(_rep + delta, 0, serviceConfig.ActiveLevel.MaxRep);
            ReputationChanged?.Invoke(_rep);
        }
    }
}
