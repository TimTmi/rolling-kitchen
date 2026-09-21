using System.Collections.Generic;
using Features.Ingredient;
using UnityEngine;

namespace Features.Interaction
{
    public class IngredientSlots
    {
        private readonly Transform _owner;
        private readonly Vector3 _defaultRotation;
        private readonly Vector3 _arrangementStart;
        private readonly Vector3 _arrangementDirection;
        private readonly int _maxSlots;

        private readonly List<Pickup.Ingredient> _slots = new();

        public IngredientSlots(Transform owner, Vector3 defaultRotation, Vector3 arrangementStart,
            Vector3 arrangementDirection, int maxSlots)
        {
            _owner = owner;
            _defaultRotation = defaultRotation;
            _arrangementStart = arrangementStart;
            _arrangementDirection = arrangementDirection;
            _maxSlots = maxSlots;
        }

        public int Capacity => _maxSlots;
        public int Count => _slots.Count;
        public bool HasFreeSlot => FindFreeSlot() >= 0;
        public Pickup.Ingredient this[int index] => _slots[index];

        public void ClearDetached()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] != null && !_slots[i].transform.IsChildOf(_owner))
                {
                    _slots[i] = null;
                }
            }
        }

        public int IndexOf(Pickup.Ingredient ingredient)
        {
            return _slots.IndexOf(ingredient);
        }

        public void Place(Pickup.Ingredient ingredient)
        {
            int slot = AcquireSlot();
            _slots[slot] = ingredient;
            ingredient.transform.SetParent(_owner);
            ingredient.transform.SetLocalPositionAndRotation(ArrangementPosition(slot), Quaternion.Euler(_defaultRotation));
        }

        public void ReplaceWithResults(int slotIndex, IngredientProcess process)
        {
            Object.Destroy(_slots[slotIndex].gameObject);
            _slots[slotIndex] = null;

            for (var i = 0; i < process.Result.Length; i++)
            {
                int slot = i == 0 ? slotIndex : AcquireSlot();
                if (slot < 0)
                {
                    break;
                }

                Pickup.Ingredient result = Object.Instantiate(process.Result[i], _owner);
                _slots[slot] = result;
                result.transform.SetLocalPositionAndRotation(ArrangementPosition(slot), Quaternion.Euler(_defaultRotation));
            }
        }

        private int FindFreeSlot()
        {
            int vacant = _slots.FindIndex(x => x == null);
            if (vacant >= 0)
            {
                return vacant;
            }

            return _slots.Count < _maxSlots ? _slots.Count : -1;
        }

        private int AcquireSlot()
        {
            int slot = FindFreeSlot();
            if (slot == _slots.Count)
            {
                _slots.Add(null);
            }

            return slot;
        }

        private Vector3 ArrangementPosition(int slot)
        {
            return _arrangementStart + _arrangementDirection * slot;
        }
    }
}
