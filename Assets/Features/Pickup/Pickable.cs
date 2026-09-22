using Features.Interaction;
using UnityEngine;

namespace Features.Pickup
{
    public class Pickable : MonoBehaviour, IInteractable
    {
        [field: SerializeField]
        public PickableData Data { get; private set; }

        public IngredientStack Stack { get; internal set; }

        public virtual bool CanInteract(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                return true;
            }

            return IngredientStack.CanMerge(context.HeldPickable, this);
        }

        public virtual void Interact(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                context.PickUp(Stack == null ? this : Stack);
                return;
            }

            IngredientStack.Merge(context.HeldPickable, this, context);
        }

        public virtual string GetInteractionPrompt(in InteractionContext context)
        {
            return Data.DisplayName;
        }
    }
}
