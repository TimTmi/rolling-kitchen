using System;
using System.Collections.Generic;
using Features.Pickables;
using Features.Service;
using UnityEngine;

namespace Features.Interaction
{
    public class FridgeDoorInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private IngredientData[] ingredients;

        public event Action<IReadOnlyList<IngredientData>> Opened;
        
        public bool CanInteract(in InteractionContext context)
        {
            return true;
        }

        public void Interact(in InteractionContext context)
        {
            Opened?.Invoke(ingredients);
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Open Fridge";
        }
    }
}
