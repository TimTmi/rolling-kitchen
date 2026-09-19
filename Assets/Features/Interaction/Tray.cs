using UnityEngine;

namespace Features.Interaction
{
    public class Tray : MonoBehaviour, IInteractable
    {
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable != null;
        }

        public void Interact(in InteractionContext context)
        {
            
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Place Item";
        }
    }
}
