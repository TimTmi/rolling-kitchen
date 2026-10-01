using Features.Dish;
using Features.Ingredients;
using Features.Service;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.GameOver
{
    public class GameOverController : UIComponent
    {
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private ServiceConfig serviceConfig;

        private Label _title;
        private Label _ordersServed;
        private Button _homeButton;
        private Button _replayButton;
        private Button _nextLevelButton;
        private VisualElement _unlockPanel;
        private VisualElement _unlockIcon;
        private Label _unlockLabel;
        private Button _unlockOkButton;

        private int _servedCount;

        protected override void OnEnabled()
        {
            orderManager.Served += OnOrderServed;
        }

        protected override void OnDisabled()
        {
            orderManager.Served -= OnOrderServed;
            Time.timeScale = 1f;
        }

        protected override void BindElements(VisualElement root)
        {
            _title = root.Q<Label>("Title");
            _ordersServed = root.Q<Label>("OrdersServed");
            _homeButton = root.Q<Button>("HomeButton");
            _replayButton = root.Q<Button>("ReplayButton");
            _nextLevelButton = root.Q<Button>("NextLevelButton");
            _unlockPanel = root.Q<VisualElement>("UnlockPanel");
            _unlockIcon = root.Q<VisualElement>("UnlockIcon");
            _unlockLabel = root.Q<Label>("UnlockLabel");
            _unlockOkButton = root.Q<Button>("UnlockOkButton");

            _unlockOkButton.clicked += HideUnlock;

            _homeButton.clicked += GameFlow.LoadMainMenu;
            _replayButton.clicked += Replay;
            _nextLevelButton.clicked += NextLevel;
        }

        protected override void Initialize()
        {
            UpdateStats();
            Hide();
        }

        public void ShowLevelFailed()
        {
            Pause();
            SetTitle("Level Failed");
            SetButtons(home: true, replay: true, next: false);
            HideUnlock();
        }

        public void ShowLevelComplete(bool hasNext)
        {
            Pause();
            SetTitle("Level Complete");
            SetButtons(home: true, replay: true, next: hasNext);
            if (hasNext)
            {
                ShowUnlock();
            }
        }

        public void ShowEndlessGameOver()
        {
            Pause();
            SaveEndlessHighScore();
            SetTitle("Game Over");
            SetButtons(home: true, replay: true, next: false);
            HideUnlock();
        }

        private void ShowUnlock()
        {
            IngredientData ingredient = serviceConfig.Levels[serviceConfig.LevelIndex + 1].UnlockedIngredient;
            if (ingredient == null)
            {
                return;
            }

            _unlockLabel.text = $"Unlocked {ingredient.DisplayName}";
            if (ingredient.Icon != null)
            {
                _unlockIcon.style.backgroundImage = new StyleBackground(ingredient.Icon);
            }

            _unlockPanel.style.display = DisplayStyle.Flex;
        }

        private void HideUnlock()
        {
            _unlockPanel.style.display = DisplayStyle.None;
        }

        private void SaveEndlessHighScore()
        {
            if (_servedCount <= PlayerPrefs.GetInt(ServiceConfig.EndlessHighScoreKey, 0))
            {
                return;
            }

            PlayerPrefs.SetInt(ServiceConfig.EndlessHighScoreKey, _servedCount);
            PlayerPrefs.Save();
        }

        private void Replay()
        {
            GameFlow.StartGame(serviceConfig, serviceConfig.GameMode, serviceConfig.LevelIndex);
        }

        private void NextLevel()
        {
            GameFlow.StartGame(serviceConfig, GameMode.Campaign, serviceConfig.LevelIndex + 1);
        }

        private void Pause()
        {
            Time.timeScale = 0f;
        }

        private void OnOrderServed(int slotIndex, Order order)
        {
            _servedCount++;
            UpdateStats();
        }

        private void SetTitle(string title)
        {
            if (_title != null)
            {
                _title.text = title;
            }
        }

        private void UpdateStats()
        {
            if (_ordersServed != null)
            {
                _ordersServed.text = $"Orders served: {_servedCount}";
            }
        }

        private void SetButtons(bool home, bool replay, bool next)
        {
            if (_homeButton != null)
            {
                _homeButton.style.display = home ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (_replayButton != null)
            {
                _replayButton.style.display = replay ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (_nextLevelButton != null)
            {
                _nextLevelButton.style.display = next ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
