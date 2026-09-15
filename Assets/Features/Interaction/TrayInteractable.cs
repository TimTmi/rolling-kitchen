using UnityEngine;

namespace Features.Interaction
{
    public class TrayInteractable : MonoBehaviour, IInteractable
    {
        public bool CanInteract(in InteractionContext context)
        {
            return true;
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
