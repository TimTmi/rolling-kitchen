using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI
{
    public abstract class UIComponent : MonoBehaviour
    {
        [SerializeField] private PanelRenderer panelRenderer;

        private int _uiVersion = -1;

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
            
            BindElements(root);
            Initialize();
        }

        protected abstract void BindElements(VisualElement root);

        protected virtual void Initialize()
        {
        }
    }
}
