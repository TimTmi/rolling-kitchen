using UnityEngine;

namespace Features.Interaction 
{
    public interface IInteractable
    {
        bool CanInteract(in InteractionContext context);
        void Interact(in InteractionContext context);
        string GetInteractionPrompt(in InteractionContext context);
    }
}