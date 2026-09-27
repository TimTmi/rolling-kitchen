using System;
using Features.Service;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.MainMenu
{
    public class MainMenuController : UIComponent
    {
        [SerializeField] private Camera menuCamera;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float rotationSpeed = 12f;

        public event Action<GameMode> PlayRequested;

        public bool MenuVisible { get; private set; }

        private Button _playButton;
        private Button _settingsButton;
        private Button _exitButton;
        private Button _campaignButton;
        private Button _endlessButton;
        private VisualElement _buttons;
        private VisualElement _modeButtons;
        private Label _title;

        protected override void OnEnabled()
        {
            EnterMenuState();
        }

        protected override void BindElements(VisualElement root)
        {
            _playButton = root.Q<Button>("PlayButton");
            _settingsButton = root.Q<Button>("SettingsButton");
            _exitButton = root.Q<Button>("ExitButton");
            _campaignButton = root.Q<Button>("CampaignButton");
            _endlessButton = root.Q<Button>("EndlessButton");
            _buttons = root.Q("Buttons");
            _modeButtons = root.Q("ModeButtons");
            _title = root.Q<Label>("Title");

            _playButton.clicked += ShowModeSelection;
            _campaignButton.clicked += () => StartGame(GameMode.Campaign);
            _endlessButton.clicked += () => StartGame(GameMode.Endless);
            _exitButton.clicked += QuitGame;
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

        private void ShowModeSelection()
        {
            _title.style.display = DisplayStyle.None;
            _buttons.style.display = DisplayStyle.None;
            _modeButtons.style.display = DisplayStyle.Flex;
        }

        private void ShowMainMenu()
        {
            if (_buttons == null || _modeButtons == null || _title == null)
            {
                return;
            }

            _title.style.display = DisplayStyle.Flex;
            _buttons.style.display = DisplayStyle.Flex;
            _modeButtons.style.display = DisplayStyle.None;
        }

        public void StartGame(GameMode gameMode)
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

            PlayRequested?.Invoke(gameMode);
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void EnterMenuState()
        {
            MenuVisible = true;
            ShowMainMenu();

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
