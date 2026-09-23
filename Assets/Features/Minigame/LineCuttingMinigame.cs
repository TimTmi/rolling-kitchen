using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Minigame
{
    public class LineCuttingMinigame : CuttingMinigame
    {
        [SerializeField] private Vector3 ingredientRotation;
        [SerializeField] private Vector3 firstLinePosition;
        [SerializeField] private Vector3 lastLinePosition;
        [SerializeField] private float cutDistance;
        [SerializeField] private int lineCount;
        [SerializeField] private GameObject knifePrefab;
        [SerializeField] private Vector3 knifeCuttingOffset;
        [SerializeField] private Vector3 knifeLiftedOffset;
        [SerializeField] private Vector3 knifeCuttingRotationOffset;
        [SerializeField] private Vector3 knifeLiftedRotationOffset;
        [SerializeField] private float knifeLeftLimit;
        [SerializeField] private float knifeRightLimit;
        [SerializeField] private float expectedInputSpeed = 1f;

        public override float ExpectedDuration => lineCount * cutDistance / Mathf.Max(expectedInputSpeed, 0.01f);

        private Transform _knife;
        private Vector3 _lineCenter;
        private float _cursorX;
        private float _distance;
        private int _line;
        private bool _cutting;

        protected override void OnStarted()
        {
            Ingredient.transform.localRotation = Quaternion.Euler(ingredientRotation);
            _knife = Instantiate(knifePrefab, transform).transform;
            _lineCenter = LinePosition(_line);
            PlaceKnife(knifeLiftedOffset, knifeLiftedRotationOffset);
        }

        protected override void Tick()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !TryGetCursorPosition(out Vector3 cursor))
            {
                return;
            }

            if (!_cutting)
            {
                if (!mouse.leftButton.wasPressedThisFrame)
                {
                    return;
                }

                _cutting = true;
                _distance = 0f;
                _cursorX = Horizontal(cursor);
                PlaceKnife(knifeCuttingOffset, knifeCuttingRotationOffset);
                return;
            }

            if (!mouse.leftButton.isPressed)
            {
                _cutting = false;
                PlaceKnife(knifeLiftedOffset, knifeLiftedRotationOffset);
                return;
            }

            float x = Horizontal(cursor);
            _distance += Mathf.Abs(x - _cursorX);
            _cursorX = x;
            PlaceKnife(knifeCuttingOffset, knifeCuttingRotationOffset);

            if (_distance < cutDistance)
            {
                return;
            }

            _line++;
            _cutting = false;
            if (_line >= lineCount)
            {
                Complete();
                return;
            }

            _lineCenter = LinePosition(_line);
            PlaceKnife(knifeLiftedOffset, knifeLiftedRotationOffset);
        }

        private float Horizontal(Vector3 cursor)
        {
            return ClampToIngredient(Vector3.Dot(cursor - _lineCenter, ViewCamera.transform.right));
        }

        private float ClampToIngredient(float x)
        {
            float center = Vector3.Dot(_lineCenter - Ingredient.transform.position, ViewCamera.transform.right);
            return Mathf.Clamp(x, knifeLeftLimit - center, knifeRightLimit - center);
        }

        private Vector3 LinePosition(int line)
        {
            float t = lineCount <= 1 ? 0f : (float)line / (lineCount - 1);
            return transform.TransformPoint(Vector3.Lerp(firstLinePosition, lastLinePosition, t));
        }

        private void PlaceKnife(Vector3 offset, Vector3 rotationOffset)
        {
            Vector3 right = ViewCamera.transform.right;
            Vector3 normal = -ViewCamera.transform.forward;
            _knife.SetPositionAndRotation(
                _lineCenter + right * _cursorX + offset,
                Quaternion.LookRotation(right, normal) * Quaternion.Euler(rotationOffset));
        }
    }
}
