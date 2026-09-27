using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.MainMenu
{
    public class MainMenuController : UIComponent
    {
        [SerializeField] private Camera menuCamera;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float rotationSpeed = 12f;

        public event Action PlayRequested;

        public bool MenuVisible { get; private set; }

        private Button _playButton;
        private Button _settingsButton;
        private Button _exitButton;

        protected override void OnEnabled()
        {
            EnterMenuState();
        }

        protected override void BindElements(VisualElement root)
        {
            _playButton = root.Q<Button>("PlayButton");
            _settingsButton = root.Q<Button>("SettingsButton");
            _exitButton = root.Q<Button>("ExitButton");

            _playButton.clicked += StartGame;
        }

        protected override void Initialize()
        {
            Show();
        }

        private void Update()
        {
            if (MenuVisible && menuCamera != null && menuCamera.enabled)
            {
                menuCamera.transform.RotateAround(menuCamera.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
            }
        }

        public void StartGame()
        {
            if (!MenuVisible)
            {
                return;
            }

            MenuVisible = false;
            Hide();

            if (menuCamera != null)
            {
                menuCamera.enabled = false;
            }

            if (playerCamera != null)
            {
                playerCamera.enabled = true;
            }

            Core.CursorController.Lock();

            PlayRequested?.Invoke();
        }

        private void EnterMenuState()
        {
            MenuVisible = true;

            if (menuCamera != null)
            {
                menuCamera.enabled = true;
            }

            if (playerCamera != null)
            {
                playerCamera.enabled = false;
            }

            Core.CursorController.Unlock();
        }
    }
}
