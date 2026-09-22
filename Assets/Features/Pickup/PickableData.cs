using UnityEngine;

namespace Features.Pickup
{
    public enum StackingRole
    {
        Container = 0,
        Base = 1,
        Topping = 2,
        Top = 3
    }

    [CreateAssetMenu(fileName = "PickableData", menuName = "Scriptable Objects/PickableData")]
    public class PickableData : ScriptableObject
    {
        [SerializeField] private string id;
        public string Id => id;

        [SerializeField] private string displayName;
        public string DisplayName => displayName;

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;

        [SerializeField] private StackingRole stackingRole = StackingRole.Topping;
        public StackingRole StackingRole => stackingRole;

        [SerializeField] private float stackHeight = 0.1f;
        public float StackHeight => stackHeight;
    }
}
