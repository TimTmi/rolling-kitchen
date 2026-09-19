using System;
using Features.Pickup;

namespace Features.Interaction
{
    public readonly struct InteractionContext
    {
        public Pickable HeldPickable { get; }
        public Action<Pickable> PickUp { get; }
        public Action Remove { get; }

        public InteractionContext(Pickable heldPickable, Action<Pickable> pickUp, Action remove)
        {
            HeldPickable = heldPickable;
            PickUp = pickUp;
            Remove = remove;
        }
    }
}