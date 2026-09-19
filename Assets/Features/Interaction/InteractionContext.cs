using System;
using Features.Pickup;

namespace Features.Interaction
{
    public readonly struct InteractionContext
    {
        public Pickable HeldPickable { get; }
        public Action<Pickable> PickUp { get; }
        public Action Remove { get; }
        public Action Release { get; }

        public InteractionContext(Pickable heldPickable, Action<Pickable> pickUp, Action remove, Action release)
        {
            HeldPickable = heldPickable;
            PickUp = pickUp;
            Remove = remove;
            Release = release;
        }
    }
}