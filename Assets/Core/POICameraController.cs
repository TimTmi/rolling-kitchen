using System;
using System.Collections;
using UnityEngine;

namespace Core
{
    public class POICameraController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera poiCamera;
        [SerializeField] private float transitionDuration = 0.5f;

        private Coroutine _transition;

        public void FocusPOI(Transform poi)
        {
            StartTransition(() => (poi.position, poi.rotation), ActivatePOICamera);
        }

        public void ReturnToPlayer()
        {
            if (!poiCamera.enabled) return;
            StartTransition(() => (playerCamera.transform.position, playerCamera.transform.rotation), ActivatePlayerCamera);
        }

        private void StartTransition(Func<(Vector3 position, Quaternion rotation)> target, Action onComplete)
        {
            if (_transition != null) StopCoroutine(_transition);
            _transition = StartCoroutine(Transition(target, onComplete));
        }

        private IEnumerator Transition(Func<(Vector3 position, Quaternion rotation)> target, Action onComplete)
        {
            if (!poiCamera.enabled)
            {
                poiCamera.transform.SetPositionAndRotation(playerCamera.transform.position, playerCamera.transform.rotation);
                poiCamera.enabled = true;
                playerCamera.enabled = false;
            }

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

        private void ActivatePOICamera()
        {
            poiCamera.enabled = true;
            playerCamera.enabled = false;
        }

        private void ActivatePlayerCamera()
        {
            poiCamera.enabled = false;
            playerCamera.enabled = true;
        }
    }
}
