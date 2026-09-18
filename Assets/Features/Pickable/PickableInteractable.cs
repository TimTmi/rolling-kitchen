using System;
using Features.Interaction;
using Features.Pickables;
using UnityEngine;

namespace Features.Pickable
{
    public class PickableInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private PickableData pickableData;

        public event Action<PickableData> PickedUp;
        
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable == null;
        }

        public void Interact(in InteractionContext context)
        {
            PickedUp?.Invoke(pickableData);
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return $"Pick Up {pickableData.DisplayName}";
        }
    }
}
