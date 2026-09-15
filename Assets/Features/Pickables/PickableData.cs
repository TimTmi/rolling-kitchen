using UnityEngine;

namespace Features.Pickables
{
    [CreateAssetMenu(fileName = "PickableData", menuName = "Scriptable Objects/PickableData")]
    public class PickableData : ScriptableObject
    {
        public string displayName;
        public Sprite icon;
        public GameObject prefab;
    }
}
