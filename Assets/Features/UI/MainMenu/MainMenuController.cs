using System;
using System.Collections.Generic;
using Features.Service;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.MainMenu
{
    public class MainMenuController : UIComponent
    {
        [SerializeField] private Camera menuCamera;
        [SerializeField] private ServiceConfig serviceConfig;
        [SerializeField] private float rotationSpeed = 12f;

        public bool MenuVisible { get; private set; }

        private Button _playButton;
        private Button _settingsButton;
        private Button _exitButton;
        private Button _campaignButton;
        private Button _endlessButton;
        private Button _backButton;
        private Button _levelBackButton;
        private VisualElement _buttons;
        private VisualElement _modeButtons;
        private VisualElement _levelButtons;
        private VisualElement _levelList;
        private Label _title;
        private int _highestCompletedLevelIndex;

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
            _backButton = root.Q<Button>("BackButton");
            _levelBackButton = root.Q<Button>("LevelBackButton");
            _levelButtons = root.Q("LevelButtons");
            _levelList = root.Q("LevelList");

            _playButton.clicked += ShowModeSelection;
            _campaignButton.clicked += ShowLevelSelection;
            _endlessButton.clicked += () => StartGame(GameMode.Endless);
            _backButton.clicked += ShowMainMenu;
            _levelBackButton.clicked += ShowModeSelection;
            _exitButton.clicked += QuitGame;
        }

        protected override void Initialize()
        {
            _highestCompletedLevelIndex = PlayerPrefs.GetInt(ServiceConfig.HighestCompletedLevelIndexKey, -1);
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
            _levelButtons.style.display = DisplayStyle.None;
            _modeButtons.style.display = DisplayStyle.Flex;
        }

        private void ShowLevelSelection()
        {
            PopulateLevelList();
            _title.style.display = DisplayStyle.None;
            _modeButtons.style.display = DisplayStyle.None;
            _levelButtons.style.display = DisplayStyle.Flex;
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
            _levelButtons.style.display = DisplayStyle.None;
        }

        private void PopulateLevelList()
        {
            _levelList.Clear();

            if (serviceConfig == null)
            {
                return;
            }

            LevelData[] levels = serviceConfig.Levels;
            for (int i = 0; i < levels.Length; i++)
            {
                int levelIndex = i;
                Button button = new Button(() => StartGame(GameMode.Campaign, levelIndex))
                {
                    text = (i + 1).ToString()
                };
                button.AddToClassList("menu-button");
                button.AddToClassList("level-button");

                if (levelIndex > _highestCompletedLevelIndex + 1)
                {
                    button.AddToClassList("locked");
                    button.SetEnabled(false);
                }

                _levelList.Add(button);
            }
        }

        public void StartGame(GameMode gameMode, int levelIndex = 0)
        {
            if (!MenuVisible)
            {
                return;
            }

            MenuVisible = false;
            GameFlow.StartGame(serviceConfig, gameMode, levelIndex);
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
            Core.CursorController.Unlock();
        }
    }
}
