using System;
using System.Collections.Generic;
using Features.Pickables;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.PickableSelection
{
    public class PickableSelectionController : UIController
    {
        [SerializeField] private VisualTreeAsset pickableSlotTemplate;

        private VisualElement _root;
        private ScrollView _list;
        private Button _putBackButton;
        
        public event Action<PickableData> PickableSelected;

        protected override void BindElements(VisualElement root)
        {
            _root = root;
            _list = root.Q<ScrollView>("List");
            _putBackButton = root.Q<Button>("PutBackButton");
        }

        protected override void Initialize()
        {
            Hide();
        }

        public void Show(IReadOnlyList<PickableData> pickables)
        {
            _list.Clear();
            
            foreach (var pickable in pickables) {
                var slot = pickableSlotTemplate.Instantiate();
                var icon = slot.Q<Image>("Icon");
                var label = slot.Q<Label>("Label");

                icon.sprite = pickable.icon;
                label.text = pickable.displayName;
                
                _list.Add(slot);
            }

            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }
    }
}
