using Features.Pickup;
using UnityEngine;

namespace Features.Player
{
    public class HandController : MonoBehaviour
    {
        public Pickable HeldPickable { get; private set; }

        public void PickUp(Pickable pickable)
        {
            Remove();
            
            HeldPickable = pickable;
            HeldPickable.transform.parent = transform;
            HeldPickable.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void Remove()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            HeldPickable = null;
        }

        public void Release()
        {
            HeldPickable = null;
        }
    }
}
