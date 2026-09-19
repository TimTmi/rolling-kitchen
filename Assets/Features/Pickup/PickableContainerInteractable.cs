using Features.Interaction;
using UnityEngine;

namespace Features.Pickup
{
    public class PickableContainerInteractable : MonoBehaviour, IInteractable
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
                context.PickUp(Instantiate(pickable).GetComponent<Pickable>());
            }
            else if (context.HeldPickable.Data == pickable.Data)
            {
                context.Remove();
            }
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                return $"Pick {pickable.Data.DisplayName} Up";
            }
            if (context.HeldPickable.Data == pickable.Data)
            {
                return $"Put {pickable.Data.DisplayName} Back";
            }

            return pickable.Data.DisplayName;
        }
    }
}
