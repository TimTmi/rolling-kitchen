using UnityEngine;

namespace Features.Pickables
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
        
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;
        
    }
}
