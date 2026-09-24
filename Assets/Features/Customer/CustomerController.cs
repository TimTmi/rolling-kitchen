using System;
using UnityEngine;

namespace Features.Customer
{
    public class CustomerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float rotationSpeed = 320f;

        private const float ReachDistance = 0.01f;

        private Transform _target;
        private Quaternion _targetRotation;
        private bool _walking;
        private bool _facing;

        public event Action Arrived;

        public void WalkTo(Transform target)
        {
            _target = target;
            _walking = true;
            _facing = false;
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
            Vector3 position = transform.position;
            Vector3 targetPosition = new Vector3(_target.position.x, position.y, _target.position.z);
            Vector3 offset = targetPosition - position;

            if (offset.magnitude <= ReachDistance)
            {
                _walking = false;
                _facing = true;
                _targetRotation = Quaternion.Euler(0, _target.eulerAngles.y, 0);
                return;
            }

            Vector3 direction = offset.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
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
    }
}
