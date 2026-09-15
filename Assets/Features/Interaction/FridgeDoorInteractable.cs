using UnityEngine;

namespace Features.Interaction
{
    public class FridgeDoorInteractable : MonoBehaviour, IInteractable
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
            return "Open Fridge";
        }
    }
}
