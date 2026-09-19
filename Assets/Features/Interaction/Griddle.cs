using Features.Ingredient;
using UnityEngine;

namespace Features.Interaction
{
    public class Griddle : MonoBehaviour, IInteractable
    {
        public bool CanInteract(in InteractionContext context)
        {
            var pickable = context.HeldPickable;
            return pickable is Pickup.Ingredient ingredient && ingredient.IngredientData.HasProcess(ProcessType.Grill);
        }

        public void Interact(in InteractionContext context)
        {
            throw new System.NotImplementedException();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Grill";
        }
    }
}