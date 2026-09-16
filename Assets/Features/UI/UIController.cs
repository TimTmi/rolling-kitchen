using UnityEngine;

namespace Features.UI
{
    public class UIController : MonoBehaviour
    {
        private UIComponent _activeComponent;
    
        public bool HasActiveComponent() => _activeComponent != null;

        public void ShowComponent(UIComponent component)
        {
            component.Show();
            _activeComponent = component;
        }

        public void HideComponent()
        {
            _activeComponent?.Hide();
            _activeComponent = null;
        }

    }
}
