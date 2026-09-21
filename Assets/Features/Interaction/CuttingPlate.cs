using Features.Ingredient;
using Features.Minigame;
using Features.Service;
using UnityEngine;

namespace Features.Interaction
{
    public class CuttingPlate : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform cameraPoint;
        [SerializeField] private ServiceController serviceController;
        [SerializeField] private Vector3 defaultRotation = new Vector3(-90f, 0f, 0f);
        [SerializeField] private Vector3 arrangementStart;
        [SerializeField] private Vector3 arrangementDirection = Vector3.right;
        [SerializeField] private int maxSlots = 8;

        private IngredientSlots _slots;
        private CuttingMinigame _minigame;

        private void Awake()
        {
            _slots = new IngredientSlots(transform, defaultRotation, arrangementStart, arrangementDirection, maxSlots);
        }

        private void OnEnable()
        {
            if (serviceController == null) return;
            serviceController.PoiFocusEnded += OnPoiFocusEnded;
        }

        private void OnDisable()
        {
            if (serviceController == null) return;
            serviceController.PoiFocusEnded -= OnPoiFocusEnded;
        }

        public bool CanInteract(in InteractionContext context)
        {
            _slots.ClearDetached();

            if (context.HeldPickable is not Pickup.Ingredient ingredient
                || !ingredient.IngredientData.HasProcess(ProcessType.Slice))
            {
                return false;
            }

            ingredient.IngredientData.TryGetProcess(ProcessType.Slice, out IngredientProcess process);
            return process.Result.Length <= _slots.Capacity && _slots.HasFreeSlot;
        }

        public void Interact(in InteractionContext context)
        {
            var ingredient = (Pickup.Ingredient)context.HeldPickable;
            _slots.Place(ingredient);

            context.Release();

            serviceController.FocusPoi(cameraPoint, true);
            StartCutting(ingredient);
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Slice";
        }

        private void StartCutting(Pickup.Ingredient ingredient)
        {
            if (!ingredient.IngredientData.TryGetProcess(ProcessType.Slice, out IngredientProcess process))
            {
                return;
            }

            if (process.Minigame == null)
            {
                Cut(ingredient);
                return;
            }

            _minigame = Instantiate(process.Minigame, transform);
            _minigame.Begin(ingredient, serviceController.PoiCamera, () =>
            {
                _minigame = null;
                Cut(ingredient);
                serviceController.ReturnToPlayer();
            });
        }

        private void OnPoiFocusEnded()
        {
            if (_minigame == null)
            {
                return;
            }

            _minigame.End();
            _minigame = null;
        }

        private void Cut(Pickup.Ingredient ingredient)
        {
            int slotIndex = _slots.IndexOf(ingredient);
            if (slotIndex < 0
                || !ingredient.IngredientData.TryGetProcess(ProcessType.Slice, out IngredientProcess process))
            {
                return;
            }

            _slots.ReplaceWithResults(slotIndex, process);
        }
    }
}
