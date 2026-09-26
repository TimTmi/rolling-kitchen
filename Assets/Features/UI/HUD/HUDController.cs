using Features.Customer;
using Features.Interaction;
using Features.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.HUD
{
    public class HUDController : UIComponent
    {
        [SerializeField] private InteractionController interactionController;
        [SerializeField] private HandController handController;
        [SerializeField] private CustomerScheduler customerScheduler;

        private Label _interactionPrompt;
        private VisualElement _crosshair;
        private Label _ordersLeftCounter;

        protected override void OnEnabled()
        {
            interactionController.FocusGained += OnFocusGained;
            interactionController.FocusLost += OnFocusLost;
            customerScheduler.OrdersLeftChanged += OnOrdersLeftChanged;
        }

        protected override void OnDisabled()
        {
            interactionController.FocusGained -= OnFocusGained;
            interactionController.FocusLost -= OnFocusLost;
            customerScheduler.OrdersLeftChanged -= OnOrdersLeftChanged;
        }

        protected override void BindElements(VisualElement root)
        {
            _interactionPrompt = root.Q<Label>("InteractionPrompt");
            _crosshair = root.Q("Crosshair");
            _ordersLeftCounter = root.Q<Label>("OrdersLeftCounter");
        }

        protected override void Initialize()
        {
            UpdateOrdersLeft(customerScheduler.OrdersLeft);
        }

        public void SetCrosshairVisible(bool visible)
        {
            if (_crosshair != null)
            {
                _crosshair.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private void OnFocusGained(IInteractable interactable)
        {
            if (_interactionPrompt == null)
            {
                return;
            }

            InteractionContext context = new(handController.HeldPickable, handController.PickUp, handController.Remove, handController.Release);

            if (!interactable.CanInteract(context))
            {
                return;
            }
            
            _interactionPrompt.visible = true;
            _interactionPrompt.text = interactable.GetInteractionPrompt(context);
        }

        private void OnOrdersLeftChanged(int ordersLeft)
        {
            UpdateOrdersLeft(ordersLeft);
        }

        private void UpdateOrdersLeft(int ordersLeft)
        {
            if (_ordersLeftCounter == null)
            {
                return;
            }

            _ordersLeftCounter.text = $"Orders left: {ordersLeft}";
        }

        private void OnFocusLost(IInteractable interactable)
        {
            if (_interactionPrompt == null)
            {
                return;
            }

            _interactionPrompt.visible = false;
        }
    }
}
