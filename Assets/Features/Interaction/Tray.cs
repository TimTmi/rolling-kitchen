using System.Collections.Generic;
using Features.Pickup;
using UnityEngine;

namespace Features.Interaction
{
    public class Tray : MonoBehaviour, IInteractable
    {
        [SerializeField] private Vector3 defaultRotation = new Vector3(-90f, 0f, 0f);
        [SerializeField] private Vector3 arrangementStart;
        [SerializeField] private Vector3 arrangementDirection = Vector3.right;
        [SerializeField] private int maxSlots = 8;

        private IngredientSlots _slots;

        private IngredientSlots Slots =>
            _slots ??= new IngredientSlots(transform, defaultRotation, arrangementStart, arrangementDirection, maxSlots);

        public IEnumerable<Pickable> Contents
        {
            get
            {
                _slots = Slots;
                _slots.ClearDetached();
                for (int i = 0; i < _slots.Count; i++)
                {
                    if (_slots[i] != null)
                    {
                        yield return _slots[i];
                    }
                }
            }
        }

        public bool CanInteract(in InteractionContext context)
        {
            _slots = Slots;
            _slots.ClearDetached();

            return context.HeldPickable != null && _slots.HasFreeSlot;
        }

        public void Interact(in InteractionContext context)
        {
            Pickable content = context.HeldPickable is IngredientStack stack ? stack.Root : context.HeldPickable;
            _slots = Slots;
            _slots.Place(content);

            context.Release();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Place";
        }
    }
}
