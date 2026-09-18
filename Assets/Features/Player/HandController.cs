using Features.Pickables;
using UnityEngine;

namespace Features.Player
{
    public class HandController : MonoBehaviour
    {
        private PickableData _heldPickableData;
        public PickableData HeldPickableData => _heldPickableData;

        public void PickUp(PickableData pickableData)
        {
            Remove();
            
            _heldPickableData = pickableData;
            GameObject instance = Instantiate(_heldPickableData.Prefab, gameObject.transform);
        }

        public void Remove()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            _heldPickableData = null;
        }
    }
}
