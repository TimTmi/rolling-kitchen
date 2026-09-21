using Features.Ingredient;
using UnityEngine;

namespace Features.Interaction
{
    public class Griddle : MonoBehaviour, IInteractable
    {
        [SerializeField] private Vector3 defaultRotation = new Vector3(-90f, 0f, 0f);
        [SerializeField] private Vector3 arrangementStart;
        [SerializeField] private Vector3 arrangementDirection = Vector3.right;
        [SerializeField] private int maxSlots = 8;

        private IngredientSlots _slots;

        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable is Pickup.Ingredient ingredient
                && ingredient.IngredientData.HasProcess(ProcessType.Grill)
                && _slots.HasFreeSlot;
        }

        public void Interact(in InteractionContext context)
        {
            var ingredient = (Pickup.Ingredient)context.HeldPickable;
            _slots.Place(ingredient);

            context.Release();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Grill";
        }

        private void Awake()
        {
            _slots = new IngredientSlots(transform, defaultRotation, arrangementStart, arrangementDirection, maxSlots);
        }

        private void Update()
        {
            _slots.ClearDetached();

            for (int i = 0; i < _slots.Count; i++)
            {
                Pickup.Ingredient ingredient = _slots[i];

                if (ingredient == null
                    || !ingredient.IngredientData.TryGetProcess(ProcessType.Grill, out IngredientProcess process)
                    || ingredient.CookingProgress >= process.Duration)
                {
                    continue;
                }

                ingredient.AddCookingProgress(Time.deltaTime);

                if (ingredient.CookingProgress >= process.Duration)
                {
                    _slots.ReplaceWithResults(i, process);
                }
            }
        }
    }
}
