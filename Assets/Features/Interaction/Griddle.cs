using System.Linq;
using Features.Audio;
using Features.Ingredients;
using Features.Pickup;
using UnityEngine;

namespace Features.Interaction
{
    public class Griddle : MonoBehaviour, IInteractable, IFryingStation
    {
        public int FryingItemCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _slots.Count; i++)
                {
                    if (_slots[i] != null)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        [SerializeField] private Vector3 defaultRotation = new Vector3(-90f, 0f, 0f);
        [SerializeField] private Vector3 arrangementStart;
        [SerializeField] private Vector3 arrangementDirection = Vector3.right;
        [SerializeField] private int maxSlots = 8;

        private IngredientSlots _slots;

        public bool CanInteract(in InteractionContext context)
        {
            bool grillable = context.HeldPickable is IngredientStack stack
                ? stack.Contents.Any(HasGrillProcess)
                : HasGrillProcess(context.HeldPickable);

            return grillable && _slots.HasFreeSlot;
        }

        public void Interact(in InteractionContext context)
        {
            Pickable content = context.HeldPickable is IngredientStack stack ? stack.Root : context.HeldPickable;
            _slots.Place(content);

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
                Pickable content = _slots[i];

                if (content != null)
                {
                    Grill(i, content);
                }
            }
        }

        private void Grill(int slotIndex, Pickable content)
        {
            IngredientStack stack = content.Stack;

            if (stack == null)
            {
                GrillBare(slotIndex, content);
                return;
            }

            foreach (Pickable member in stack.Contents.ToList())
            {
                if (member == null
                    || member.Stack != stack
                    || member is not Ingredient ingredient
                    || !ingredient.HasUnfinishedProcess(ProcessType.Grill, out IngredientProcess process))
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
                    _slots.ReplaceRoot(slotIndex, process.Result);
                }
                else
                {
                    stack.Replace(member, process.Result);
                }

                AudioController.PlayDing();
                break;
            }
        }

        private void GrillBare(int slotIndex, Pickable content)
        {
            if (content is not Ingredient ingredient
                || !ingredient.HasUnfinishedProcess(ProcessType.Grill, out IngredientProcess process))
            {
                return;
            }

            ingredient.AddCookingProgress(Time.deltaTime);

            if (ingredient.CookingProgress >= process.Duration)
            {
                _slots.ReplaceWithResults(slotIndex, process);
                AudioController.PlayDing();
            }
        }

        private static bool HasGrillProcess(Pickable content)
        {
            return content is Ingredient ingredient && ingredient.IngredientData.HasProcess(ProcessType.Grill);
        }
    }
}
