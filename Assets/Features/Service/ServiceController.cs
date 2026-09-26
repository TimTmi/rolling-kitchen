using System;
using System.Collections.Generic;
using Features.Customer;
using Features.Interaction;
using Features.Pickup;
using Features.Player;
using Features.Reputation;
using Features.UI;
using Features.UI.GameOver;
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
        [SerializeField] private GameOverController gameOver;
        [SerializeField] private ServiceConfig serviceConfig;
        [SerializeField] private CustomerScheduler customerScheduler;
        [SerializeField] private ReputationController reputationController;
        [SerializeField] private Fridge fridge;
        [SerializeField] private PickableSelectionController pickableSelectionController;
        [SerializeField] private Core.PoiCameraController poiCameraController;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera poiCamera;

        private Action<Pickable> _selectionHandler;
        private bool _poiFocusCancellable;
        private bool _gameOver;

        public event Action PoiFocusEnded;

        public Camera PoiCamera => poiCamera;

        private void Start()
        {
            Core.CursorController.Lock();
            fridge.LimitIngredients(serviceConfig.CurrentLevel.GetIngredients());
        }

        private void OnEnable()
        {
            fridge.Opened += OnFridgeOpened;

            pickableSelectionController.CloseRequested += HideUIComponent;
            pickableSelectionController.PickableSelected += OnPickableSelected;

            poiCameraController.PoiFocusStarted += OnPoiFocusStarted;
            poiCameraController.PlayerFocusEnded += OnPlayerFocusEnded;

            reputationController.ReputationChanged += OnReputationChanged;
            customerScheduler.OrdersCompleted += OnOrdersCompleted;
        }

        private void OnDisable()
        {
            fridge.Opened -= OnFridgeOpened;
            pickableSelectionController.CloseRequested -= HideUIComponent;
            pickableSelectionController.PickableSelected -= OnPickableSelected;
            poiCameraController.PoiFocusStarted -= OnPoiFocusStarted;
            poiCameraController.PlayerFocusEnded -= OnPlayerFocusEnded;

            reputationController.ReputationChanged -= OnReputationChanged;
            customerScheduler.OrdersCompleted -= OnOrdersCompleted;
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
            if (!context.performed || _gameOver)
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

        private void OnReputationChanged(int rep)
        {
            if (_gameOver || rep > 0)
            {
                return;
            }

            _gameOver = true;
            DisablePlayerControl();
            if (serviceConfig.GameMode == GameMode.Endless)
            {
                gameOver.ShowEndlessGameOver();
            }
            else
            {
                gameOver.ShowLevelFailed();
            }

            uiController.ShowComponent(gameOver);
        }

        private void OnOrdersCompleted()
        {
            if (_gameOver || serviceConfig.GameMode != GameMode.Campaign)
            {
                return;
            }

            _gameOver = true;
            DisablePlayerControl();
            gameOver.ShowLevelComplete(serviceConfig.LevelIndex + 1 < serviceConfig.Levels.Length);
            uiController.ShowComponent(gameOver);
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
