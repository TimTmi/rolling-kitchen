using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        
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
            
            _focusedInteractable.Interact();
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
                Debug.Log(hit.collider.name);
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                Debug.Log(interactable);
                if (interactable != null)
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
