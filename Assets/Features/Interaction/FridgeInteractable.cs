using System;
using System.Collections.Generic;
using Features.Ingredient;
using UnityEngine;

namespace Features.Interaction
{
    public class FridgeInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Pickup.Ingredient[] ingredients;

        public event Action<IReadOnlyList<Pickup.Ingredient>> Opened;
        public event Action PutBackRequested;
        
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
            else if  (context.HeldPickable is Pickup.Ingredient)
            {
                PutBackRequested?.Invoke();
            }
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                return "Open Fridge";
            }
            else if (context.HeldPickable is Pickup.Ingredient)
            {
                return $"Put {context.HeldPickable.Data.DisplayName} Back";
            }
            else
            {
                return "Fridge";
            }
        }
    }
}
