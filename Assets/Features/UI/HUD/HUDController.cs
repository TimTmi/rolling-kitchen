using Features.Interaction;
using Features.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.HUD
{
    public class HUDController : UIComponent
    {
        [SerializeField] private InteractionController interactionController;
        [SerializeField] private PlayerController playerController;
        
        private Label _interactionPrompt;

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
        }

        private void OnFocusGained(IInteractable interactable)
        {
            _interactionPrompt.visible = true;
            _interactionPrompt.text = interactable.GetInteractionPrompt(new(playerController.HeldPickable));
        }

        private void OnFocusLost(IInteractable interactable)
        {
            _interactionPrompt.visible = false;
        }
    }
}
