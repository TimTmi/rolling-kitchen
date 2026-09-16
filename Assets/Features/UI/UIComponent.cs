using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI
{
    public abstract class UIComponent : MonoBehaviour
    {
        [SerializeField] private PanelRenderer panelRenderer;

        private int _uiVersion = -1;
        private VisualElement _root;

        private void OnEnable()
        {
            _uiVersion = -1;
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
            OnEnabled();
        }

        protected virtual void OnEnabled()
        {
        }

        private void OnDisable()
        {
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
            OnDisabled();
        }

        protected virtual void OnDisabled()
        {
        }

        private void OnUIReload(PanelRenderer _, VisualElement root, int version)
        {
            if (version <= _uiVersion)
            {
                return;
            }
            _uiVersion = version;
            _root = root;
            
            BindElements(root);
            Initialize();
        }

        protected abstract void BindElements(VisualElement root);

        protected virtual void Initialize()
        {
        }

        public virtual void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public virtual void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }
    }
}
