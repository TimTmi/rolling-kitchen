using System;
using Features.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private HandController handController;

        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private float interactionDistance = 2f;

        public event Action<IInteractable> FocusGained;
        public event Action<IInteractable> FocusLost;
        public event Action<IInteractable> Interacted;

        private IInteractable _focusedInteractable;

        public void OnInteract(InputAction.CallbackContext context)
        {
            InteractionContext interactionContext = new(handController.HeldPickableData);

            if (!context.performed || _focusedInteractable == null || !_focusedInteractable.CanInteract(interactionContext))
            {
                return;
            }
            
            var interactable = _focusedInteractable;
            interactable.Interact(interactionContext);
            LoseFocus();

            Interacted?.Invoke(interactable);
        }
        
        private void Update()
        {
            if (Physics.Raycast(
                    camera.transform.position,
                    camera.transform.forward,
                    out RaycastHit hit,
                    interactionDistance,
                    interactionLayer,
                    QueryTriggerInteraction.Ignore))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    GainFocus(interactable);
                    return;
                }
            }

            LoseFocus();
        }

        private void GainFocus(IInteractable interactable)
        {
            if (_focusedInteractable == interactable)
            {
                return;
            }
            
            IInteractable previousInteractable = _focusedInteractable;
            _focusedInteractable = interactable;

            if (previousInteractable != null)
            {
                FocusLost?.Invoke(previousInteractable);
            }
            
            FocusGained?.Invoke(interactable);
        }

        private void LoseFocus()
        {
            if (_focusedInteractable == null)
            {
                return;
            }
            
            IInteractable previousInteractable = _focusedInteractable;
            _focusedInteractable = null;
            FocusLost?.Invoke(previousInteractable);
        }
    }
}
