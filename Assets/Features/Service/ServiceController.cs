using System;
using System.Collections.Generic;
using Features.Interaction;
using Features.Pickable;
using Features.Pickables;
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
        [SerializeField] private FridgeDoorInteractable fridgeDoorInteractable;
        [SerializeField] private PickableContainerInteractable burgerBoxStackInteractable;
        [SerializeField] private PickableContainerInteractable friesBoxStackInteractable;
        [SerializeField] private PickableSelectionController  pickableSelectionController;

        private Action<PickableData> _selectionHandler;
        
        void Start()
        {
            Core.CursorController.Lock();
        }

        private void OnEnable()
        {
            fridgeDoorInteractable.Opened += OnFridgeOpened;
            fridgeDoorInteractable.PutBackRequested += OnPickablePutBackRequested;
            pickableSelectionController.CloseRequested += HideUIComponent;
            pickableSelectionController.PickableSelected += OnPickableSelected;
            
            burgerBoxStackInteractable.PickUpRequested += OnPickablePickUpRequested;
            friesBoxStackInteractable.PickUpRequested += OnPickablePickUpRequested;
            burgerBoxStackInteractable.PutBackRequested += OnPickablePutBackRequested;
            friesBoxStackInteractable.PutBackRequested += OnPickablePutBackRequested;
        }

        private void OnDisable()
        {
            fridgeDoorInteractable.Opened -= OnFridgeOpened;
            fridgeDoorInteractable.PutBackRequested -= OnPickablePutBackRequested;
            pickableSelectionController.CloseRequested -= HideUIComponent;
            burgerBoxStackInteractable.PickUpRequested -= OnPickablePickUpRequested;
            friesBoxStackInteractable.PickUpRequested -= OnPickablePickUpRequested;
            burgerBoxStackInteractable.PutBackRequested -= OnPickablePutBackRequested;
            friesBoxStackInteractable.PutBackRequested -= OnPickablePutBackRequested;
        }
        
        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            HideUIComponent();
        }

        private void OnFridgeOpened(IReadOnlyList<PickableData> pickables)
        {
            ShowPickableSelection(pickables, (data => handController.PickUp(data)));
        }

        private void ShowPickableSelection(IReadOnlyList<PickableData> pickables, Action<PickableData> selectionHandler)
        {
            _selectionHandler = selectionHandler;
            
            pickableSelectionController.SetPickables(pickables);
            ShowUIComponent(pickableSelectionController);
        }

        private void OnPickableSelected(PickableData pickable)
        {
            HideUIComponent();
            
            _selectionHandler?.Invoke(pickable);
            _selectionHandler = null;
        }

        private void ShowUIComponent(UIComponent component)
        {
            interactionController.enabled = false;
            playerInput.SwitchCurrentActionMap("UI");
            Core.CursorController.Unlock();
            uiController.ShowComponent(component);
        }

        private void HideUIComponent()
        {
            playerInput.SwitchCurrentActionMap("Player");
            Core.CursorController.Lock();
            uiController.HideComponent();
            interactionController.enabled = true;
        }

        private void OnPickablePickUpRequested(PickableData pickableData)
        {
            handController.PickUp(pickableData);
        }

        private void OnPickablePutBackRequested()
        {
            handController.Remove();
        }
    }
}
