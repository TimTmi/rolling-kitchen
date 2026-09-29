using System;
using Features.Service;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.Pause
{
    public class PauseController : UIComponent
    {
        [SerializeField] private ServiceConfig serviceConfig;

        public event Action CloseRequested;

        private Button _continueButton;
        private Button _restartButton;
        private Button _settingsButton;
        private Button _homeButton;
        private Button _exitButton;
        private VisualElement _buttons;
        private VisualElement _settingsPanel;
        private Slider _sensitivitySlider;
        private Button _resetSensitivityButton;
        private Button _resetDataButton;
        private Button _settingsBackButton;

        protected override void OnDisabled()
        {
            Time.timeScale = 1f;
        }

        protected override void BindElements(VisualElement root)
        {
            _continueButton = root.Q<Button>("ContinueButton");
            _restartButton = root.Q<Button>("RestartButton");
            _settingsButton = root.Q<Button>("SettingsButton");
            _homeButton = root.Q<Button>("HomeButton");
            _exitButton = root.Q<Button>("ExitButton");
            _buttons = root.Q("Buttons");
            _settingsPanel = root.Q("SettingsPanel");
            _sensitivitySlider = root.Q<Slider>("SensitivitySlider");
            _resetSensitivityButton = root.Q<Button>("ResetSensitivityButton");
            _resetDataButton = root.Q<Button>("ResetDataButton");
            _settingsBackButton = root.Q<Button>("SettingsBackButton");

            _continueButton.clicked += () => CloseRequested?.Invoke();
            _restartButton.clicked += Restart;
            _settingsButton.clicked += ShowSettings;
            _homeButton.clicked += GameFlow.LoadMainMenu;
            _exitButton.clicked += QuitGame;
            _sensitivitySlider.RegisterValueChangedCallback(evt => SaveSensitivity(evt.newValue));
            _resetSensitivityButton.clicked += ResetSensitivity;
            _resetDataButton.clicked += ResetData;
            _settingsBackButton.clicked += ShowButtons;
        }

        protected override void Initialize()
        {
            ShowButtons();
            Hide();
        }

        private void ShowButtons()
        {
            _buttons.style.display = DisplayStyle.Flex;
            _settingsPanel.style.display = DisplayStyle.None;
        }

        private void ShowSettings()
        {
            _sensitivitySlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(ServiceConfig.CameraSensitivityKey, ServiceConfig.DefaultCameraSensitivity));
            _buttons.style.display = DisplayStyle.None;
            _settingsPanel.style.display = DisplayStyle.Flex;
        }

        private void SaveSensitivity(float value)
        {
            PlayerPrefs.SetFloat(ServiceConfig.CameraSensitivityKey, value);
            PlayerPrefs.Save();
        }

        private void ResetSensitivity()
        {
            _sensitivitySlider.value = ServiceConfig.DefaultCameraSensitivity;
        }

        private void ResetData()
        {
            PlayerPrefs.DeleteKey(ServiceConfig.HighestCompletedLevelIndexKey);
            PlayerPrefs.DeleteKey(ServiceConfig.EndlessHighScoreKey);
            PlayerPrefs.Save();
        }

        private void Restart()
        {
            GameFlow.StartGame(serviceConfig, serviceConfig.GameMode, serviceConfig.LevelIndex);
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
