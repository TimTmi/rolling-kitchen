using System;
using Features.Customer;
using Features.Dish;
using UnityEngine;

namespace Features.Reputation
{
    public class ReputationController : MonoBehaviour
    {
        [SerializeField] private CustomerScheduler customerScheduler;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private int maxRep = 100;
        [SerializeField] private int repLoss = -5;
        [SerializeField] private int repGain = 2;

        private int _rep;

        public event Action<int> ReputationChanged;

        public int Rep => _rep;
        public int MaxRep => maxRep;

        private void Awake()
        {
            _rep = maxRep;
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
            AddRep(repLoss);
        }

        private void OnOrderServed(int slotIndex, Order order)
        {
            AddRep(repGain);
        }

        private void AddRep(int delta)
        {
            _rep = Mathf.Clamp(_rep + delta, 0, maxRep);
            ReputationChanged?.Invoke(_rep);
        }
    }
}
