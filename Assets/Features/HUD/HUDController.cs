using Features.Interaction;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.HUD
{
    [RequireComponent(typeof(PanelRenderer))]
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private InteractionController interactionController;
        
        private PanelRenderer _panelRenderer;
        private int _uiVersion = 0;
        
        private Label _interactionPrompt;
        
        private void Awake()
        {
            _panelRenderer = GetComponent<PanelRenderer>();
        }

        private void OnEnable()
        {
            _panelRenderer.RegisterUIReloadCallback(OnUIReload);
            interactionController.FocusGained += OnFocusGained;
            interactionController.FocusLost += OnFocusLost;
        }

        private void OnDisable()
        {
            _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
            interactionController.FocusGained -= OnFocusGained;
            interactionController.FocusLost -= OnFocusLost;
        }

        private void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
        {
            if (version <= _uiVersion)
            {
                return;
            }
            _uiVersion = version;
            
            _interactionPrompt = root.Q<Label>("InteractionPrompt");
        }

        private void OnFocusGained(IInteractable interactable)
        {
            _interactionPrompt.visible = true;
            _interactionPrompt.text = interactable.GetInteractionPrompt();
        }

        private void OnFocusLost(IInteractable interactable)
        {
            _interactionPrompt.visible = false;
        }
    }
}
