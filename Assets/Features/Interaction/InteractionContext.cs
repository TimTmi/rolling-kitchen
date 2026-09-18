using Features.Pickables;

namespace Features.Interaction
{
    public readonly struct InteractionContext
    {
        public readonly PickableData HeldPickable;

        public InteractionContext(PickableData heldPickable)
        {
            HeldPickable = heldPickable;
        }
    }
}