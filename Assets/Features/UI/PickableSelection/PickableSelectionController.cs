using System;
using System.Collections.Generic;
using Features.Pickables;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.PickableSelection
{
    public class PickableSelectionController : UIComponent
    {
        [SerializeField] private VisualTreeAsset pickableSlotTemplate;

        private ScrollView _list;
        private Button _putBackButton;
        private Button _closeButton;

        public event Action<PickableData> PickableSelected;
        public event Action CloseRequested;

        protected override void BindElements(VisualElement root)
        {
            _list = root.Q<ScrollView>("List");
            _putBackButton = root.Q<Button>("PutBackButton");
            _closeButton = root.Q<Button>("CloseButton");

            _closeButton.clicked += () => CloseRequested?.Invoke();
        }

        protected override void Initialize()
        {
            Hide();
        }

        public void SetPickables(IReadOnlyList<PickableData> pickables)
        {
            _list.Clear();
            
            foreach (var pickable in pickables) {
                var slot = pickableSlotTemplate.Instantiate();
                slot.AddToClassList("pickable-slot-item");
                var button = slot.Q<Button>("PickableSlot");
                var icon = slot.Q<Image>("Icon");
                var label = slot.Q<Label>("Label");

                icon.sprite = pickable.icon;
                label.text = pickable.displayName;

                button.clicked += () => PickableSelected?.Invoke(pickable);

                _list.Add(slot);
            }
        }
    }
}
