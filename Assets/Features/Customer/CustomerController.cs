using System;
using Features.Dish;
using Features.Interaction;
using UnityEngine;

namespace Features.Customer
{
    public class CustomerController : MonoBehaviour, IInteractable
    {
        private static readonly int Speed = Animator.StringToHash("Speed");

        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float rotationSpeed = 320f;
        [SerializeField] private Animator animator;

        private const float ReachDistance = 0.01f;

        private OrderManager _orderManager;
        private int _slotIndex;
        private Transform _target;
        private Quaternion _targetRotation;
        private bool _walking;
        private bool _facing;

        public event Action Arrived;

        public void Init(OrderManager orderManager, int slotIndex)
        {
            _orderManager = orderManager;
            _slotIndex = slotIndex;
        }

        public void WalkTo(Transform target)
        {
            _target = target;
            _walking = true;
            _facing = false;
            animator.SetFloat(Speed, 1f);
        }

        private void Update()
        {
            if (_walking)
            {
                WalkStep();
                return;
            }

            if (_facing)
            {
                FaceStep();
            }
        }

        private void WalkStep()
        {
            Vector3 offset = _target.position - transform.position;

            if (offset.magnitude <= ReachDistance)
            {
                transform.position = _target.position;
                _walking = false;
                _facing = true;
                animator.SetFloat(Speed, 0f);
                _targetRotation = Quaternion.Euler(0, _target.eulerAngles.y, 0);
                return;
            }

            Vector3 direction = offset.normalized;
            Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
            if (flatDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }

            transform.position += direction * (walkSpeed * Time.deltaTime);
        }

        private void FaceStep()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, _targetRotation) > 0.01f)
            {
                return;
            }

            transform.rotation = _targetRotation;
            _facing = false;
            Arrived?.Invoke();
        }

        public bool CanInteract(in InteractionContext context)
        {
            return _orderManager != null && _orderManager.GetOrder(_slotIndex) != null;
        }

        public void Interact(in InteractionContext context)
        {
            _orderManager.TryServe(_slotIndex);
        }

        public string GetInteractionPrompt(in InteractionContext context)
        {
            return "Serve";
        }
    }
}
