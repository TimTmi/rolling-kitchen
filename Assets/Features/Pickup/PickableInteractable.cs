using Features.Interaction;
using UnityEngine;

namespace Features.Pickup
{
    public class PickableInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Pickup.Pickable pickable;
        
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable == null;
        }

        public void Interact(in InteractionContext context)
        {
            throw new System.NotImplementedException();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return $"Pick Up {pickable.name}";
        }
    }
}