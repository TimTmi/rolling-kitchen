using UnityEngine;

namespace Features.Pickup
{
    [CreateAssetMenu(fileName = "PickableData", menuName = "Scriptable Objects/PickableData")]
    public class PickableData : ScriptableObject
    {
        [SerializeField] private string id;
        public string Id => id;
        
        [SerializeField] private string displayName;
        public string DisplayName => displayName;
        
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
    }
}
