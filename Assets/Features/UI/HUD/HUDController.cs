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
        
        private Label _interactionPrompt;
        private VisualElement _crosshair;

        protected override void OnEnabled()
        {
            interactionController.FocusGained += OnFocusGained;
            interactionController.FocusLost += OnFocusLost;
        }

        protected override void OnDisabled()
        {
            interactionController.FocusGained -= OnFocusGained;
            interactionController.FocusLost -= OnFocusLost;
        }

        protected override void BindElements(VisualElement root)
        {
            _interactionPrompt = root.Q<Label>("InteractionPrompt");
            _crosshair = root.Q("Crosshair");
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
            InteractionContext context = new(handController.HeldPickable, handController.PickUp, handController.Remove, handController.Release);

            if (!interactable.CanInteract(context))
            {
                return;
            }
            
            _interactionPrompt.visible = true;
            _interactionPrompt.text = interactable.GetInteractionPrompt(context);
        }

        private void OnFocusLost(IInteractable interactable)
        {
            _interactionPrompt.visible = false;
        }
    }
}
