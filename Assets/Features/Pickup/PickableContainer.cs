using Features.Interaction;
using UnityEngine;

namespace Features.Pickup
{
    public class PickableContainer : MonoBehaviour, IInteractable
    {
        [SerializeField] private Pickable pickable;
        
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable == null || context.HeldPickable.Data == pickable.Data;
        }

        public void Interact(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                context.PickUp(Instantiate(pickable));
            }
            else if (context.HeldPickable.Data == pickable.Data)
            {
                context.Remove();
            }
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            if (context.HeldPickable != null && context.HeldPickable.Data == pickable.Data)
            {
                return "Put Back";
            }

            return pickable.Data.DisplayName;
        }
    }
}
