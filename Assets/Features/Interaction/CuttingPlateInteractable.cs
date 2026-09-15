using UnityEngine;

namespace Features.Interaction
{
    public class CuttingPlateInteractable : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            throw new System.NotImplementedException();
        }

        public string GetInteractionPrompt()
        {
            return "cutting board";
        }
    }
}
