using System;
using System.Collections.Generic;
using Features.Ingredient;
using UnityEngine;

namespace Features.Interaction
{
    public class Fridge : MonoBehaviour, IInteractable
    {
        [SerializeField] private Pickup.Ingredient[] ingredients;

        public event Action<IReadOnlyList<Pickup.Ingredient>> Opened;
        
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable == null || context.HeldPickable is Pickup.Ingredient;
        }

        public void Interact(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                Opened?.Invoke(ingredients);
            }
            else if (context.HeldPickable is Pickup.Ingredient)
            {
                context.Remove();
            }
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                return "Open Fridge";
            }
            if (context.HeldPickable is Pickup.Ingredient)
            {
                return $"Put Back";
            }

            return "Fridge";
        }
    }
}
