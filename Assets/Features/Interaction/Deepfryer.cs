using UnityEngine;

namespace Features.Interaction
{
    public class Deepfryer : MonoBehaviour, IInteractable
    {
        public bool CanInteract(in InteractionContext context)
        {
            throw new System.NotImplementedException();
        }

        public void Interact(in InteractionContext context)
        {
            throw new System.NotImplementedException();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Fry";
        }
    }
}