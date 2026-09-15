using Features.Pickables;
using UnityEngine;

namespace Features.Interaction
{
    public class CuttingPlateInteractable : MonoBehaviour, IInteractable
    {
        public bool CanInteract(in InteractionContext context)
        {
            var pickable = context.HeldPickable;

            return pickable is IngredientData ingredientData &&
                ingredientData.HasProcess(ProcessType.Slice);
        }
        
        public void Interact(in  InteractionContext context)
        {
            
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Slice";
        }
    }
}
