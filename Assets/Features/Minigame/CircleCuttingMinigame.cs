using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Minigame
{
    public class CircleCuttingMinigame : CuttingMinigame
    {
        [SerializeField] private GameObject knifePrefab;
        [SerializeField] private Vector3 knifeRotationOffset = new(0f, 90f, 0f);
        [SerializeField] private float knifeRaise = 0.02f;
        [SerializeField] private float knifeFollowSpeed = 720f;

        private Transform _knife;
        private Vector3 _center;
        private float _radius;
        private float _angle;
        private float _swept;

        protected override void OnStarted()
        {
            _center = Ingredient.transform.position;
            _radius = ComputeBounds(Ingredient.gameObject).extents.magnitude + 0.02f;

            _knife = Instantiate(knifePrefab, transform).transform;
            FitKnifeToOrbit();
        }

        protected override void Tick()
        {
            Mouse mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.isPressed)
            {
                _swept += Mathf.Min(FollowKnifeByMouseAngle(), 90f);
            }

            if (_swept >= 360f)
            {
                Complete();
            }
        }

        private float FollowKnifeByMouseAngle()
        {
            if (!TryGetCursorPosition(out Vector3 cursor))
            {
                return 0f;
            }

            Vector3 offset = cursor - _center;
            float target = Mathf.Atan2(
                Vector3.Dot(offset, ViewCamera.transform.up),
                Vector3.Dot(offset, ViewCamera.transform.right)) * Mathf.Rad2Deg;
            float previous = _angle;
            _angle = Mathf.MoveTowardsAngle(_angle, target, knifeFollowSpeed * Time.deltaTime);
            float lastFrameSwept = Mathf.Abs(Mathf.DeltaAngle(previous, _angle));

            float radians = _angle * Mathf.Deg2Rad;
            Vector3 direction = ViewCamera.transform.right * Mathf.Cos(radians)
                                + ViewCamera.transform.up * Mathf.Sin(radians);
            Vector3 normal = -ViewCamera.transform.forward;
            _knife.SetPositionAndRotation(
                _center + direction * _radius + normal * knifeRaise,
                Quaternion.LookRotation(direction, normal) * Quaternion.Euler(knifeRotationOffset));

            return lastFrameSwept;
        }

        private void FitKnifeToOrbit()
        {
            Bounds bounds = ComputeBounds(_knife.gameObject);
            float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (size > 0f)
            {
                _knife.localScale *= _radius * 1.5f / size;
            }
        }

        private static Bounds ComputeBounds(GameObject root)
        {
            Bounds bounds = new(root.transform.position, Vector3.zero);
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }
    }
}
