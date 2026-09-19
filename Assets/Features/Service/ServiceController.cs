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
        [SerializeField] private FridgeInteractable fridgeInteractable;
        [SerializeField] private PickableContainerInteractable burgerBoxStackInteractable;
        [SerializeField] private PickableContainerInteractable friesBoxStackInteractable;
        [SerializeField] private BinInteractable binInteractable;
        [SerializeField] private PickableSelectionController  pickableSelectionController;

        private Action<Pickable> _selectionHandler;
        
        void Start()
        {
            Core.CursorController.Lock();
        }

        private void OnEnable()
        {
            fridgeInteractable.Opened += OnFridgeOpened;
            
            pickableSelectionController.CloseRequested += HideUIComponent;
            pickableSelectionController.PickableSelected += OnPickableSelected;
        }

        private void OnDisable()
        {
            fridgeInteractable.Opened -= OnFridgeOpened;
            pickableSelectionController.CloseRequested -= HideUIComponent;
        }
        
        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            HideUIComponent();
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
    }
}
