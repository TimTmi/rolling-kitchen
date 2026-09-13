using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        
        [SerializeField] private float interactionDistance = 0.6f;

        public event Action<IInteractable> FocusGained;
        public event Action<IInteractable> FocusLost;
        public event Action<IInteractable> Interacted;

        private IInteractable _focusedInteractable;

        public void OnInteract(InputAction.CallbackContext context)
        {
            Interacted?.Invoke(_focusedInteractable);
        }
        
        private void Update()
        {
            if (Physics.Raycast(
                    camera.transform.position,
                    camera.transform.forward,
                    out RaycastHit hit,
                    interactionDistance))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
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
            }
            else if (_focusedInteractable != null)
            {
                IInteractable previousInteractable = _focusedInteractable;
                _focusedInteractable = null;
                FocusLost?.Invoke(previousInteractable);
            }
        }
    }
}
