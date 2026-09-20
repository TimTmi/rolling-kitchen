using Core;
using Features.Ingredient;
using UnityEngine;

namespace Features.Interaction
{
    public class CuttingPlate : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform cameraPoint;
        [SerializeField] private POICameraController camera;
        
        public bool CanInteract(in InteractionContext context)
        {
            var pickable = context.HeldPickable;
            
            if (context.HeldPickable is not Pickup.Ingredient)
            {
                return false;
            }

            var data = (IngredientData)pickable.Data;
            return data.HasProcess(ProcessType.Slice);
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
