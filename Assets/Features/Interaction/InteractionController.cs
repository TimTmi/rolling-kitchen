using System;
using Features.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private PlayerController playerController;

        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private float interactionDistance = 2f;

        public event Action<IInteractable> FocusGained;
        public event Action<IInteractable> FocusLost;
        public event Action<IInteractable> Interacted;

        private IInteractable _focusedInteractable;

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.started || _focusedInteractable == null)
            {
                return;
            }
            
            _focusedInteractable.Interact(new(playerController.HeldPickable));
            _focusedInteractable = null;
            Interacted?.Invoke(_focusedInteractable);
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
            if (_focusedInteractable == null)
            {
                _focusedInteractable = interactable;
                FocusGained?.Invoke(interactable);
            }
            else if (interactable != _focusedInteractable)
            {
                IInteractable previousInteractable = _focusedInteractable;
                _focusedInteractable = interactable;
                FocusLost?.Invoke(previousInteractable);
                FocusGained?.Invoke(_focusedInteractable);
            }
        }

        private void LoseFocus()
        {
            IInteractable previousInteractable = _focusedInteractable;
            _focusedInteractable = null;
            FocusLost?.Invoke(previousInteractable);
        }
    }
}
