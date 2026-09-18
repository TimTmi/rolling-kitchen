using System;
using Features.Interaction;
using Features.Pickables;
using UnityEngine;

namespace Features.Pickable
{
    public class PickableContainerInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private PickableData pickableData;

        public event Action<PickableData> PickUpRequested;
        public event Action PutBackRequested;
        
        public bool CanInteract(in InteractionContext context)
        {
            return context.HeldPickable == null || context.HeldPickable == pickableData;
        }

        public void Interact(in InteractionContext context)
        {
            if (context.HeldPickable == null)
            {
                PickUpRequested?.Invoke(pickableData);
            }
            else if (context.HeldPickable == pickableData)
            {
                PutBackRequested?.Invoke();
            }
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            string action = "";
            
            if (context.HeldPickable == null)
            {
                action = "Pick Up";
            }
            else if (context.HeldPickable == pickableData)
            {
                action = "Put Back";
            }

            return $"{action} {pickableData.DisplayName}".Trim();
        }
    }
}
