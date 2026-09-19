using Features.Ingredient;
using Features.Interaction;

namespace Features.Pickup
{
    public class Ingredient : Pickable, IInteractable
    {
        public IngredientData IngredientData => (IngredientData)Data;
        public float CookingProgress { get; private set; }
        
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable == null;
        }

        public void Interact(in InteractionContext context)
        {
            context.PickUp(this);
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return $"Pick {context.HeldPickable.Data.DisplayName} Up";
        }
    }
}