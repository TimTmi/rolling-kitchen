using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Camera camera;
    
        [SerializeField] private float minPitch = -60f;
        [SerializeField] private float maxPitch = 60f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float rotationSpeed = 32f;
        [SerializeField] private float gravity = -2f;
    
        private Vector2 _moveInput;
        private Vector2 _lookInput;

        private float _pitch;
        private float _verticalVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }
    
        private void Update()
        {
            _verticalVelocity += Time.deltaTime * gravity;
            if (characterController.isGrounded && gravity < 0)
                _verticalVelocity = gravity;
        
            Vector3 move = transform.right * _moveInput.x + transform.up * _verticalVelocity + transform.forward * _moveInput.y;
            characterController.Move(Time.deltaTime * moveSpeed * move);
        
            transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed *  _lookInput.x);

            _pitch -= Time.deltaTime * rotationSpeed * _lookInput.y;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
            camera.transform.localRotation = Quaternion.Euler(_pitch, 0, 0);
        }
    }
}
