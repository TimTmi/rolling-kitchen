using System;
using System.Collections;
using UnityEngine;

namespace Core
{
    public class PoiCameraController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera poiCamera;
        [SerializeField] private float transitionDuration = 0.5f;

        private Coroutine _transition;
        private bool _focused;

        public event Action PoiFocusStarted;
        public event Action PlayerFocusEnded;

        public void FocusPoi(Transform poi)
        {
            bool wasFocused = _focused;
            _focused = true;

            if (!wasFocused)
            {
                poiCamera.transform.SetPositionAndRotation(playerCamera.transform.position, playerCamera.transform.rotation);
                PoiFocusStarted?.Invoke();
            }

            StartTransition(() => (poi.position, poi.rotation));
        }

        public void ReturnToPlayer()
        {
            if (!_focused) return;
            _focused = false;

            StartTransition(() => (playerCamera.transform.position, playerCamera.transform.rotation), PlayerFocusEnded);
        }

        private void StartTransition(Func<(Vector3 position, Quaternion rotation)> target, Action onComplete = null)
        {
            if (_transition != null) StopCoroutine(_transition);
            _transition = StartCoroutine(Transition(target, onComplete));
        }

        private IEnumerator Transition(Func<(Vector3 position, Quaternion rotation)> target, Action onComplete)
        {
            Vector3 fromPosition = poiCamera.transform.position;
            Quaternion fromRotation = poiCamera.transform.rotation;
            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
                (Vector3 position, Quaternion rotation) = target();
                poiCamera.transform.SetPositionAndRotation(
                    Vector3.Lerp(fromPosition, position, t),
                    Quaternion.Slerp(fromRotation, rotation, t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            (Vector3 endPosition, Quaternion endRotation) = target();
            poiCamera.transform.SetPositionAndRotation(endPosition, endRotation);
            onComplete?.Invoke();
            _transition = null;
        }
    }
}
