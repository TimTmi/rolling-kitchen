using Features.Interaction;
using UnityEngine;

namespace Features.Pickup
{
    public class PickableContainer : MonoBehaviour, IInteractable
    {
        [SerializeField] private Pickable pickable;
        
        public bool CanInteract(in InteractionContext context)
        {
            if (context.HeldPickable == null || context.HeldPickable.Data == pickable.Data)
            {
                return true;
            }

            return IngredientStack.CanMergeInto(pickable, context.HeldPickable);
        }

        public void Interact(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                context.PickUp(Instantiate(pickable));
                return;
            }

            if (context.HeldPickable.Data == pickable.Data)
            {
                context.Remove();
                return;
            }

            IngredientStack.MergeInto(Instantiate(pickable), context.HeldPickable);
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
