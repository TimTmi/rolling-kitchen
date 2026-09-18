using System;
using System.Collections.Generic;
using Features.Pickables;
using Features.Service;
using UnityEngine;

namespace Features.Interaction
{
    public class FridgeInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private IngredientData[] ingredients;

        public event Action<IReadOnlyList<IngredientData>> Opened;
        public event Action PutBackRequested;
        
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable == null || context.HeldPickable is IngredientData;
        }

        public void Interact(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                Opened?.Invoke(ingredients);
            }
            else if  (context.HeldPickable is IngredientData)
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
            else if (context.HeldPickable is IngredientData)
            {
                return $"Put {context.HeldPickable.DisplayName} Back";
            }
            else
            {
                return "Fridge";
            }
        }
    }
}
