using UnityEngine;

namespace Features.Interaction
{
    public class TrayInteractable : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            throw new System.NotImplementedException();
        }

        public string GetInteractionPrompt()
        {
            return "tray";
        }
    }
}
