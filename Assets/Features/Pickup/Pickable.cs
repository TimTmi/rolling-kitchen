using Features.Interaction;
using UnityEngine;

namespace Features.Pickup
{
    public class Pickable : MonoBehaviour, IInteractable
    {
        [field: SerializeField]
        public PickableData Data { get; private set; }

        public virtual bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable == null;
        }

        public virtual void Interact(in InteractionContext context)
        {
            context.PickUp(this);
        }

        public virtual string GetInteractionPrompt(in InteractionContext context)
        {
            return Data.DisplayName;
        }
    }
}
