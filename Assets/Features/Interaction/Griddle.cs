using System.Collections.Generic;
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

        private readonly List<Pickup.Ingredient> _cooking = new();

        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable is Pickup.Ingredient ingredient
                && ingredient.IngredientData.HasProcess(ProcessType.Grill)
                && FindFreeSlot() >= 0;
        }

        public void Interact(in InteractionContext context)
        {
            var ingredient = (Pickup.Ingredient)context.HeldPickable;

            int slot = AcquireSlot();
            _cooking[slot] = ingredient;
            ingredient.transform.SetParent(transform);
            ingredient.transform.SetLocalPositionAndRotation(ArrangementPosition(slot), Quaternion.Euler(defaultRotation));

            context.Release();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Grill";
        }

        private void Update()
        {
            for (int i = _cooking.Count - 1; i >= 0; i--)
            {
                Pickup.Ingredient ingredient = _cooking[i];

                if (ingredient == null)
                {
                    continue;
                }

                if (!ingredient.transform.IsChildOf(transform))
                {
                    _cooking[i] = null;
                    continue;
                }

                if (!ingredient.IngredientData.TryGetProcess(ProcessType.Grill, out IngredientProcess process)
                    || ingredient.CookingProgress >= process.Duration)
                {
                    continue;
                }

                ingredient.AddCookingProgress(Time.deltaTime);

                if (ingredient.CookingProgress >= process.Duration)
                {
                    ReplaceWithResults(i, ingredient, process);
                }
            }
        }

        private int FindFreeSlot()
        {
            int vacant = _cooking.FindIndex(x => x == null);
            if (vacant >= 0)
            {
                return vacant;
            }

            return _cooking.Count < maxSlots ? _cooking.Count : -1;
        }

        private int AcquireSlot()
        {
            int slot = FindFreeSlot();
            if (slot == _cooking.Count)
            {
                _cooking.Add(null);
            }

            return slot;
        }

        private Vector3 ArrangementPosition(int slot)
        {
            return arrangementStart + arrangementDirection * slot;
        }

        private void ReplaceWithResults(int slotIndex, Pickup.Ingredient ingredient, IngredientProcess process)
        {
            _cooking[slotIndex] = null;
            Destroy(ingredient.gameObject);

            for (var i = 0; i < process.Result.Length; i++)
            {
                var slot = AcquireSlot();
                if (slot < 0)
                {
                    break;
                }

                Pickup.Ingredient result = Instantiate(process.Result[i], transform);
                _cooking[slot] = result;
                result.transform.SetLocalPositionAndRotation(ArrangementPosition(slot), Quaternion.Euler(defaultRotation));
            }
        }
    }
}
