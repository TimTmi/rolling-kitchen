using System;
using Features.Pickup;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Minigame
{
    public abstract class CuttingMinigame : MonoBehaviour
    {
        private Action _completed;
        private bool _running;

        protected Ingredient Ingredient { get; private set; }
        protected Camera ViewCamera { get; private set; }

        public void Begin(Ingredient ingredient, Camera viewCamera, Action completed)
        {
            Ingredient = ingredient;
            ViewCamera = viewCamera;
            _completed = completed;
            _running = true;

            OnStarted();
        }

        public void End()
        {
            Destroy(gameObject);
        }

        public abstract float ExpectedDuration { get; }

        protected abstract void OnStarted();

        protected abstract void Tick();

        protected void Complete()
        {
            if (!_running)
            {
                return;
            }

            _running = false;

            Action completed = _completed;
            End();
            completed?.Invoke();
        }

        protected bool TryGetCursorPosition(out Vector3 position)
        {
            position = default;
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return false;
            }

            Ray ray = ViewCamera.ScreenPointToRay(mouse.position.ReadValue());
            Plane plane = new(-ViewCamera.transform.forward, Ingredient.transform.position);
            if (!plane.Raycast(ray, out float distance))
            {
                return false;
            }

            position = ray.GetPoint(distance);
            return true;
        }

        private void Update()
        {
            if (_running)
            {
                Tick();
            }
        }
    }
}
