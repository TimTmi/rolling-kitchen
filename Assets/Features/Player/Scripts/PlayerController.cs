using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -2f;

    private CharacterController _characterController;
    private Vector3 _moveInput;
    private float _verticalVelocity;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
    
    private void Update()
    {
        _verticalVelocity += gravity * Time.deltaTime;
        if (_characterController.isGrounded && gravity < 0)
            _verticalVelocity = gravity;
        
        Vector3 move = new(_moveInput.x, _verticalVelocity, _moveInput.y);
        _characterController.Move(Time.deltaTime * moveSpeed * move);
    }
}
