using Features.Ingredient;
using UnityEngine;

namespace Features.Interaction
{
    public class Deepfryer : MonoBehaviour, IInteractable
    {
        [SerializeField] private Vector3 defaultRotation = new Vector3(-90f, 0f, 0f);
        [SerializeField] private Vector3 fryPosition;

        private Pickup.Ingredient _frying;

        public bool CanInteract(in InteractionContext context)
        {
            if (_frying != null)
            {
                return context.HeldPickable == null;
            }

            return context.HeldPickable is Pickup.Ingredient ingredient
                && ingredient.IngredientData.HasProcess(ProcessType.DeepFry);
        }

        public void Interact(in InteractionContext context)
        {
            if (_frying != null)
            {
                context.PickUp(_frying);
                _frying = null;
                return;
            }

            _frying = (Pickup.Ingredient)context.HeldPickable;
            _frying.transform.SetParent(transform);
            _frying.transform.SetLocalPositionAndRotation(fryPosition, Quaternion.Euler(defaultRotation));

            context.Release();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return _frying == null ? "Fry" : "Pick Up";
        }

        private void Update()
        {
            if (_frying == null || !_frying.transform.IsChildOf(transform))
            {
                _frying = null;
                return;
            }

            if (!_frying.IngredientData.TryGetProcess(ProcessType.DeepFry, out IngredientProcess process)
                || _frying.CookingProgress >= process.Duration)
            {
                return;
            }

            _frying.AddCookingProgress(Time.deltaTime);

            if (_frying.CookingProgress >= process.Duration)
            {
                ReplaceWithResult(_frying, process);
            }
        }

        private void ReplaceWithResult(Pickup.Ingredient ingredient, IngredientProcess process)
        {
            Destroy(ingredient.gameObject);

            if (process.Result.Length == 0)
            {
                _frying = null;
                return;
            }

            _frying = Instantiate(process.Result[0], transform);
            _frying.transform.SetLocalPositionAndRotation(fryPosition, Quaternion.Euler(defaultRotation));
        }
    }
}
