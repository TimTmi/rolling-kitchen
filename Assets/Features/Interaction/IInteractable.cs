using UnityEngine;

namespace Features.Interaction 
{
    public interface IInteractable 
    {
        void Interact();
        string GetInteractionMessage();
    }
}