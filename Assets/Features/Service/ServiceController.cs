using System;
using System.Collections.Generic;
using System.Linq;
using Features.Customer;
using Features.Dish;
using Features.Interaction;
using Features.Pickup;
using Features.Player;
using Features.UI;
using Features.UI.PickableSelection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Service
{
    public class ServiceController : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private InteractionController interactionController;
        [SerializeField] private HandController handController;
        [SerializeField] private UIController uiController;
        [SerializeField] private UI.HUD.HUDController hud;
        [SerializeField] private Fridge fridge;
        [SerializeField] private PickableSelectionController pickableSelectionController;
        [SerializeField] private Core.PoiCameraController poiCameraController;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera poiCamera;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private DishData[] dishes = Array.Empty<DishData>();
        [SerializeField] private float minFreeTime = 5f;
        [SerializeField] private float maxFreeTime = 10f;
        [SerializeField] private int maxOrderSize = 1;
        [SerializeField] private OrderSlotPath[] orderSlotPaths = Array.Empty<OrderSlotPath>();
        [SerializeField] private CustomerController customerPrefab;

        private Action<Pickable> _selectionHandler;
        private bool _poiFocusCancellable;
        private float[] _spawnTimers;
        private CustomerController[] _customers;
        private SlotState[] _slotStates;

        private enum SlotState
        {
            Free,
            Incoming,
            Returning
        }

        public event Action PoiFocusEnded;

        public Camera PoiCamera => poiCamera;

        public OrderSlotPath GetSlotPath(int slotIndex)
        {
            return orderSlotPaths[slotIndex];
        }

        [Serializable]
        public class OrderSlotPath
        {
            [SerializeField] private Transform pathStart;
            [SerializeField] private Transform pathEnd;

            public Transform PathStart => pathStart;
            public Transform PathEnd => pathEnd;
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

        private void SendCustomerToCounter(int slotIndex)
        {
            _slotStates[slotIndex] = SlotState.Incoming;
            OrderSlotPath path = orderSlotPaths[slotIndex];
            CustomerController customer = _customers[slotIndex];
            customer.transform.position = path.PathStart.position;
            customer.WalkTo(path.PathEnd);
        }

        private void OnCustomerArrived(int slotIndex)
        {
            if (_slotStates[slotIndex] == SlotState.Incoming)
            {
                Order order = GenerateRandomOrder();
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
            OrderSlotPath path = orderSlotPaths[slotIndex];
            _customers[slotIndex].WalkTo(path.PathStart);
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

        void Start()
        {
            Core.CursorController.Lock();
            SpawnCustomers();
        }

        private void SpawnCustomers()
        {
            _customers = new CustomerController[orderManager.SlotCount];
            _slotStates = new SlotState[orderManager.SlotCount];
            for (int i = 0; i < _customers.Length; i++)
            {
                OrderSlotPath path = orderSlotPaths[i];
                CustomerController customer = Instantiate(customerPrefab, transform);
                customer.Init(orderManager, i);
                customer.transform.position = path.PathStart.position;
                int slotIndex = i;
                customer.Arrived += () => OnCustomerArrived(slotIndex);
                _customers[i] = customer;
            }
        }

        private void OnEnable()
        {
            fridge.Opened += OnFridgeOpened;
            orderManager.Served += OnOrderServed;

            pickableSelectionController.CloseRequested += HideUIComponent;
            pickableSelectionController.PickableSelected += OnPickableSelected;

            poiCameraController.PoiFocusStarted += OnPoiFocusStarted;
            poiCameraController.PlayerFocusEnded += OnPlayerFocusEnded;
        }

        private void OnDisable()
        {
            fridge.Opened -= OnFridgeOpened;
            orderManager.Served -= OnOrderServed;
            pickableSelectionController.CloseRequested -= HideUIComponent;
            pickableSelectionController.PickableSelected -= OnPickableSelected;
            poiCameraController.PoiFocusStarted -= OnPoiFocusStarted;
            poiCameraController.PlayerFocusEnded -= OnPlayerFocusEnded;
        }

        private void OnPoiFocusStarted()
        {
            DisablePlayerControl();
            poiCamera.enabled = true;
            playerCamera.enabled = false;
            hud.SetCrosshairVisible(false);
        }

        private void OnPlayerFocusEnded()
        {
            poiCamera.enabled = false;
            playerCamera.enabled = true;
            EnablePlayerControl();
            hud.SetCrosshairVisible(true);
            PoiFocusEnded?.Invoke();
        }
        
        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            if (_poiFocusCancellable)
            {
                ReturnToPlayer();
                return;
            }

            HideUIComponent();
        }

        public void FocusPoi(Transform poi, bool cancellable)
        {
            _poiFocusCancellable = cancellable;
            poiCameraController.FocusPoi(poi);
        }

        public void ReturnToPlayer()
        {
            _poiFocusCancellable = false;
            poiCameraController.ReturnToPlayer();
        }

        private void OnFridgeOpened(IReadOnlyList<Pickable> pickables)
        {
            ShowPickableSelection(pickables, (pickable => handController.PickUp(Instantiate(pickable).GetComponent<Pickable>())));
        }

        private void ShowPickableSelection(IReadOnlyList<Pickable> pickables, Action<Pickable> selectionHandler)
        {
            _selectionHandler = selectionHandler;
            
            pickableSelectionController.SetPickables(pickables);
            ShowUIComponent(pickableSelectionController);
        }

        private void OnPickableSelected(Pickable pickable)
        {
            HideUIComponent();
            
            _selectionHandler?.Invoke(pickable);
            _selectionHandler = null;
        }

        private void ShowUIComponent(UIComponent component)
        {
            DisablePlayerControl();
            uiController.ShowComponent(component);
        }

        private void HideUIComponent()
        {
            uiController.HideComponent();
            EnablePlayerControl();
        }

        private void DisablePlayerControl()
        {
            interactionController.enabled = false;
            playerInput.SwitchCurrentActionMap("UI");
            Core.CursorController.Unlock();
        }

        private void EnablePlayerControl()
        {
            playerInput.SwitchCurrentActionMap("Player");
            Core.CursorController.Lock();
            interactionController.enabled = true;
        }
    }
}
