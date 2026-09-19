using System;
using UnityEngine;

namespace Features.Interaction
{
    public class Bin : MonoBehaviour, IInteractable
    {
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable != null;
        }

        public void Interact(in InteractionContext context)
        {
            context.Remove();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return $"Throw {context.HeldPickable.Data.DisplayName} Away";
        }
    }
}
