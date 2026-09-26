using Features.Dish;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.GameOver
{
    public class GameOverController : UIComponent
    {
        [SerializeField] private OrderManager orderManager;

        private Label _title;
        private Label _ordersServed;
        private Button _homeButton;
        private Button _replayButton;
        private Button _nextLevelButton;

        private int _servedCount;

        protected override void OnEnabled()
        {
            orderManager.Served += OnOrderServed;
        }

        protected override void OnDisabled()
        {
            orderManager.Served -= OnOrderServed;
        }

        protected override void BindElements(VisualElement root)
        {
            _title = root.Q<Label>("Title");
            _ordersServed = root.Q<Label>("OrdersServed");
            _homeButton = root.Q<Button>("HomeButton");
            _replayButton = root.Q<Button>("ReplayButton");
            _nextLevelButton = root.Q<Button>("NextLevelButton");
        }

        protected override void Initialize()
        {
            UpdateStats();
            Hide();
        }

        public void ShowLevelFailed()
        {
            SetTitle("Level Failed");
            SetButtons(home: true, replay: true, next: false);
        }

        public void ShowLevelComplete(bool hasNext)
        {
            SetTitle("Level Complete");
            SetButtons(home: true, replay: true, next: hasNext);
        }

        public void ShowEndlessGameOver()
        {
            SetTitle("Game Over");
            SetButtons(home: true, replay: true, next: false);
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
