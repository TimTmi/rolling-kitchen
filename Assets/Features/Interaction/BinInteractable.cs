using System;
using UnityEngine;

namespace Features.Interaction
{
    public class BinInteractable : MonoBehaviour, IInteractable
    {
        public event Action ThrowAwayRequeted;
        
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable != null;
        }

        public void Interact(in InteractionContext context)
        {
            ThrowAwayRequeted?.Invoke();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return $"Throw {context.HeldPickable.DisplayName} Away";
        }
    }
}
