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

        private readonly List<Pickup.Ingredient> _cooking = new();

        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable is Pickup.Ingredient;
        }

        public void Interact(in InteractionContext context)
        {
            var ingredient = (Pickup.Ingredient)context.HeldPickable;

            ingredient.transform.SetParent(transform);
            ingredient.transform.SetLocalPositionAndRotation(ArrangementPosition(_cooking.Count), Quaternion.Euler(defaultRotation));
            _cooking.Add(ingredient);

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

                if (ingredient == null || !ingredient.transform.IsChildOf(transform))
                {
                    _cooking.RemoveAt(i);
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
                    ReplaceWithResults(ingredient, process);
                }
            }
        }

        private Vector3 ArrangementPosition(int slot)
        {
            return arrangementStart + arrangementDirection * slot;
        }

        private void ReplaceWithResults(Pickup.Ingredient ingredient, IngredientProcess process)
        {
            int slot = _cooking.IndexOf(ingredient);
            _cooking.Remove(ingredient);
            Destroy(ingredient.gameObject);

            for (int i = 0; i < process.Result.Length; i++)
            {
                Pickup.Ingredient result = Instantiate(process.Result[i], transform);
                result.transform.SetLocalPositionAndRotation(ArrangementPosition(slot + i), Quaternion.Euler(defaultRotation));
                _cooking.Add(result);
            }
        }
    }
}
