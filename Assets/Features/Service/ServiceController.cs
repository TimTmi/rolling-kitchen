using System;
using System.Collections.Generic;
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
        [SerializeField] private Fridge fridge;
        [SerializeField] private PickableSelectionController  pickableSelectionController;
        [SerializeField] private Core.PoiCameraController poiCameraController;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera poiCamera;

        private Action<Pickable> _selectionHandler;
        private bool _poiFocusCancellable;

        void Start()
        {
            Core.CursorController.Lock();
        }

        private void OnEnable()
        {
            fridge.Opened += OnFridgeOpened;

            pickableSelectionController.CloseRequested += HideUIComponent;
            pickableSelectionController.PickableSelected += OnPickableSelected;

            poiCameraController.PoiFocusStarted += OnPoiFocusStarted;
            poiCameraController.PlayerFocusEnded += OnPlayerFocusEnded;
        }

        private void OnDisable()
        {
            fridge.Opened -= OnFridgeOpened;
            pickableSelectionController.CloseRequested -= HideUIComponent;
            poiCameraController.PoiFocusStarted -= OnPoiFocusStarted;
            poiCameraController.PlayerFocusEnded -= OnPlayerFocusEnded;
        }

        private void OnPoiFocusStarted()
        {
            DisablePlayerControl();
            poiCamera.enabled = true;
            playerCamera.enabled = false;
        }

        private void OnPlayerFocusEnded()
        {
            poiCamera.enabled = false;
            playerCamera.enabled = true;
            EnablePlayerControl();
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
