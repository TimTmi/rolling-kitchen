using System.Collections.Generic;
using Core;
using Features.Ingredient;
using UnityEngine;

namespace Features.Interaction
{
    public class CuttingPlate : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform cameraPoint;
        [SerializeField] private POICameraController camera;
        [SerializeField] private Vector3 defaultRotation = new Vector3(-90f, 0f, 0f);
        [SerializeField] private Vector3 arrangementStart;
        [SerializeField] private Vector3 arrangementDirection = Vector3.right;
        [SerializeField] private int maxSlots = 8;

        private readonly List<Pickup.Ingredient> _slicing = new();

        public bool CanInteract(in InteractionContext context)
        {
            for (int i = _slicing.Count - 1; i >= 0; i--)
            {
                if (_slicing[i] == null)
                {
                    continue;
                }

                if (!_slicing[i].transform.IsChildOf(transform))
                {
                    _slicing[i] = null;
                }
            }

            if (context.HeldPickable is not Pickup.Ingredient ingredient
                || !ingredient.IngredientData.HasProcess(ProcessType.Slice))
            {
                return false;
            }

            ingredient.IngredientData.TryGetProcess(ProcessType.Slice, out IngredientProcess process);
            return process.Result.Length <= maxSlots && FindFreeSlot() >= 0;
        }

        public void Interact(in InteractionContext context)
        {
            var ingredient = (Pickup.Ingredient)context.HeldPickable;

            int slot = AcquireSlot();
            _slicing[slot] = ingredient;
            ingredient.transform.SetParent(transform);
            ingredient.transform.SetLocalPositionAndRotation(ArrangementPosition(slot), Quaternion.Euler(defaultRotation));

            context.Release();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Slice";
        }

        private void Cut(Pickup.Ingredient ingredient)
        {
            int slotIndex = _slicing.IndexOf(ingredient);
            if (slotIndex < 0
                || !ingredient.IngredientData.TryGetProcess(ProcessType.Slice, out IngredientProcess process))
            {
                return;
            }

            _slicing[slotIndex] = null;
            Destroy(ingredient.gameObject);

            for (var i = 0; i < process.Result.Length; i++)
            {
                var slot = i == 0 ? slotIndex : AcquireSlot();
                if (slot < 0)
                {
                    slot = _slicing.Count;
                    _slicing.Add(null);
                }

                Pickup.Ingredient result = Instantiate(process.Result[i], transform);
                _slicing[slot] = result;
                result.transform.SetLocalPositionAndRotation(ArrangementPosition(slot), Quaternion.Euler(defaultRotation));
            }
        }

        private int FindFreeSlot()
        {
            int vacant = _slicing.FindIndex(x => x == null);
            if (vacant >= 0)
            {
                return vacant;
            }

            return _slicing.Count < maxSlots ? _slicing.Count : -1;
        }

        private int AcquireSlot()
        {
            int slot = FindFreeSlot();
            if (slot == _slicing.Count)
            {
                _slicing.Add(null);
            }

            return slot;
        }

        private Vector3 ArrangementPosition(int slot)
        {
            return arrangementStart + arrangementDirection * slot;
        }
    }
}
