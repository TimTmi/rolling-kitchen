using Core;
using Features.Ingredient;
using UnityEngine;

namespace Features.Interaction
{
    public class CuttingPlate : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform cameraPoint;
        [SerializeField] private POICameraController camera;
        [SerializeField] private Vector3 cutPosition;
        [SerializeField] private Vector3 defaultRotation = new Vector3(0f, 0f, 0f);
        [SerializeField] private Vector3 arrangementStart;
        [SerializeField] private Vector3 arrangementDirection = Vector3.right;
        [SerializeField] private float sliceDelay = 1f;

        private Pickup.Ingredient _slicing;
        private float _sliceTimer;

        public bool CanInteract(in InteractionContext context)
        {
            if (_slicing != null)
            {
                return false;
            }

            return context.HeldPickable is Pickup.Ingredient ingredient
                && ingredient.IngredientData.HasProcess(ProcessType.Slice);
        }

        public void Interact(in InteractionContext context)
        {
            _slicing = (Pickup.Ingredient)context.HeldPickable;
            _sliceTimer = 0f;
            _slicing.transform.SetParent(transform);
            _slicing.transform.SetLocalPositionAndRotation(cutPosition, Quaternion.Euler(defaultRotation));

            context.Release();
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Slice";
        }

        private void Update()
        {
            if (_slicing == null || !_slicing.transform.IsChildOf(transform))
            {
                _slicing = null;
                return;
            }

            _sliceTimer += Time.deltaTime;
            if (_sliceTimer < sliceDelay)
            {
                return;
            }

            Cut(_slicing);
        }

        private void Cut(Pickup.Ingredient ingredient)
        {
            _slicing = null;

            if (!ingredient.IngredientData.TryGetProcess(ProcessType.Slice, out IngredientProcess process))
            {
                return;
            }

            Destroy(ingredient.gameObject);

            for (var i = 0; i < process.Result.Length; i++)
            {
                Pickup.Ingredient result = Instantiate(process.Result[i], transform);
                result.transform.SetLocalPositionAndRotation(ArrangementPosition(i), Quaternion.Euler(defaultRotation));
            }
        }

        private Vector3 ArrangementPosition(int slot)
        {
            return arrangementStart + arrangementDirection * slot;
        }
    }
}
