using UnityEngine;

namespace Features.Interaction
{
    public class FridgeDoorInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform hinge;

        [SerializeField] private Vector3 openRotation;

        private Quaternion _closedRotation;
        private bool _open = false;

        private void Awake()
        {
            _closedRotation = Quaternion.Euler(hinge.localRotation.eulerAngles);
        }
        
        public void Interact()
        {
            _open = !_open;
            hinge.localRotation = _open ? Quaternion.Euler(openRotation) : _closedRotation;
        }

        public string GetInteractionMessage()
        {
            return _open ? "Close Fridge" : "Open Fridge";
        }
    }
}
