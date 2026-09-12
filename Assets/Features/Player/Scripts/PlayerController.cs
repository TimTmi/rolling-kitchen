using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float gravity = -2f;

    private CharacterController _characterController;
    
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    
    private float _verticalVelocity;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
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
        _verticalVelocity += gravity * Time.deltaTime;
        if (_characterController.isGrounded && gravity < 0)
            _verticalVelocity = gravity;
        
        Vector3 move = transform.right * _moveInput.x + transform.up * _verticalVelocity + transform.forward * _moveInput.y;
        _characterController.Move(Time.deltaTime * moveSpeed * move);
        
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime *  _lookInput.x);
    }
}
