using System.Linq;
using Features.Ingredient;
using Features.Pickup;
using UnityEngine;

namespace Features.Interaction
{
    public class Deepfryer : MonoBehaviour, IInteractable
    {
        [SerializeField] private Vector3 defaultRotation = new Vector3(-90f, 0f, 0f);
        [SerializeField] private Vector3 fryPosition;
        [SerializeField] private Vector3 submergedOffset = Vector3.zero;
        [SerializeField] private Vector3 raisedOffset = new Vector3(0f, 0.1f, 0f);

        private const float BasketSpeed = 0.5f;

        private Pickable _frying;
        private Vector3 _initialLocalPosition;

        public bool CanInteract(in InteractionContext context)
        {
            if (_frying != null)
            {
                return context.HeldPickable == null;
            }

            return context.HeldPickable is IngredientStack stack
                ? stack.Contents.Any(HasDeepFryProcess)
                : HasDeepFryProcess(context.HeldPickable);
        }

        public void Interact(in InteractionContext context)
        {
            if (_frying != null)
            {
                context.PickUp(_frying);
                _frying = null;
                return;
            }

            _frying = context.HeldPickable is IngredientStack stack ? stack.Root : context.HeldPickable;
            _frying.transform.SetParent(transform);
            _frying.transform.SetLocalPositionAndRotation(fryPosition, Quaternion.Euler(defaultRotation));

            context.Release();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return _frying == null ? "Fry" : "Pick Up";
        }

        private void Awake()
        {
            _initialLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            Vector3 basketTarget = _initialLocalPosition + (_frying != null ? submergedOffset : raisedOffset);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, basketTarget, BasketSpeed * Time.deltaTime);

            if (_frying == null || !_frying.transform.IsChildOf(transform))
            {
                _frying = null;
                return;
            }

            if (_frying.Stack == null)
            {
                Fry(_frying);
                return;
            }

            IngredientStack stack = _frying.Stack;

            foreach (Pickable member in stack.Contents.ToList())
            {
                if (member == null
                    || member.Stack != stack
                    || member is not Pickup.Ingredient ingredient
                    || !ingredient.HasUnfinishedProcess(ProcessType.DeepFry, out IngredientProcess process))
                {
                    continue;
                }

                ingredient.AddCookingProgress(Time.deltaTime);

                if (ingredient.CookingProgress < process.Duration)
                {
                    continue;
                }

                if (member == stack.Root)
                {
                    _frying = stack.ReplaceRoot(process.Result);
                }
                else
                {
                    stack.Replace(member, process.Result);
                }

                break;
            }
        }

        private void Fry(Pickable content)
        {
            if (content is not Pickup.Ingredient ingredient
                || !ingredient.HasUnfinishedProcess(ProcessType.DeepFry, out IngredientProcess process))
            {
                return;
            }

            ingredient.AddCookingProgress(Time.deltaTime);

            if (ingredient.CookingProgress >= process.Duration)
            {
                ReplaceWithResult(ingredient, process);
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

        private static bool HasDeepFryProcess(Pickable content)
        {
            return content is Pickup.Ingredient ingredient && ingredient.IngredientData.HasProcess(ProcessType.DeepFry);
        }
    }
}
