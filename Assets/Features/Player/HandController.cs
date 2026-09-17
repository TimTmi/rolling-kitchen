using Features.Pickables;
using UnityEngine;

namespace Features.Player
{
    public class HandController : MonoBehaviour
    {
        private PickableData _heldPickableData;
        public PickableData HeldPickableData => _heldPickableData;
        
        private GameObject _heldPrefab;
        public GameObject HeldPrefab => _heldPrefab;
    }
}
