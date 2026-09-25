using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Minigame
{
    public class CircleCuttingMinigame : CuttingMinigame
    {
        [SerializeField] private GameObject knifePrefab;
        [SerializeField] private Vector3 knifeRotationOffset = new(0f, 0f, 0f);
        [SerializeField] private Vector3 knifeHoverRotationOffset = new(0f, 0f, 0f);
        [SerializeField] private float knifeRaise = 0.02f;
        [SerializeField] private float knifeFollowSpeed = 720f;
        [SerializeField] private float expectedSweepSpeed = 360f;

        public override float ExpectedDuration =>
            Mathf.Max(360f / Mathf.Max(expectedSweepSpeed, 1f), 360f / knifeFollowSpeed);

        private Transform _knife;
        private Vector3 _center;
        private float _radius;
        private float _angle;
        private float _swept;
        private bool _clickedOnce;

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
            if (mouse == null || !TryGetCursorPosition(out Vector3 cursor))
            {
                return;
            }

            float target = AngleOf(cursor);

            if (mouse.leftButton.wasPressedThisFrame)
            {
                _clickedOnce = true;
                _angle = target;
                PlaceKnife(_radius, knifeRotationOffset);
                return;
            }

            if (!_clickedOnce)
            {
                HoverKnife(target);
                return;
            }

            if (!mouse.leftButton.isPressed)
            {
                return;
            }

            _swept += Mathf.Min(MoveKnife(target), 90f);

            if (_swept >= 360f)
            {
                Audio.AudioController.PlaySlicing();
                Complete();
            }
        }

        private void HoverKnife(float target)
        {
            _angle = target;
            PlaceKnife(_radius * 2f, knifeHoverRotationOffset);
        }

        private float AngleOf(Vector3 cursor)
        {
            Vector3 offset = cursor - _center;
            return Mathf.Atan2(
                Vector3.Dot(offset, ViewCamera.transform.up),
                Vector3.Dot(offset, ViewCamera.transform.right)) * Mathf.Rad2Deg;
        }

        private float MoveKnife(float target)
        {
            float delta = Mathf.DeltaAngle(_angle, target);
            if (delta > 0f)
            {
                return 0f;
            }

            float step = Mathf.MoveTowards(0f, delta, knifeFollowSpeed * Time.deltaTime);
            _angle += step;
            PlaceKnife(_radius, knifeRotationOffset);

            return -step;
        }

        private void PlaceKnife(float radius, Vector3 rotationOffset)
        {
            float radians = _angle * Mathf.Deg2Rad;
            Vector3 direction = ViewCamera.transform.right * Mathf.Cos(radians)
                                + ViewCamera.transform.up * Mathf.Sin(radians);
            Vector3 normal = -ViewCamera.transform.forward;
            _knife.SetPositionAndRotation(
                _center + direction * radius + normal * knifeRaise,
                Quaternion.LookRotation(direction, normal) * Quaternion.Euler(rotationOffset));
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
