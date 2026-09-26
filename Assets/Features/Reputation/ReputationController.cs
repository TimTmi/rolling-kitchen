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

        public event Action<int> ReputationChanged;

        public int Rep => _rep;
        public int MaxRep => serviceConfig.CurrentLevel.MaxRep;

        private void Awake()
        {
            _rep = serviceConfig.CurrentLevel.MaxRep;
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
            AddRep(serviceConfig.CurrentLevel.RepLoss);
        }

        private void OnOrderServed(int slotIndex, Order order)
        {
            AddRep(serviceConfig.CurrentLevel.RepGain);
        }

        private void AddRep(int delta)
        {
            _rep = Mathf.Clamp(_rep + delta, 0, serviceConfig.CurrentLevel.MaxRep);
            ReputationChanged?.Invoke(_rep);
        }
    }
}
