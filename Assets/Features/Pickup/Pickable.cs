using UnityEngine;

namespace Features.Pickup
{
    public class Pickable : MonoBehaviour
    {
        [field: SerializeField]
        public PickableData Data { get; private set; }
    }
}