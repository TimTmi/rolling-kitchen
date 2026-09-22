using System.Collections.Generic;
using Features.Ingredient;
using Features.Pickup;
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

        private readonly List<Pickable> _slots = new();

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
        public Pickable this[int index] => _slots[index];

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

        public int IndexOf(Pickable content)
        {
            return _slots.IndexOf(content);
        }

        public void Place(Pickable content)
        {
            int slot = AcquireSlot();
            _slots[slot] = content;
            content.transform.SetParent(_owner);
            content.transform.SetLocalPositionAndRotation(ArrangementPosition(slot), Quaternion.Euler(_defaultRotation));
        }

        public void ReplaceWithResults(int slotIndex, IngredientProcess process)
        {
            if (_slots[slotIndex] is not Pickup.Ingredient ingredient)
            {
                return;
            }

            Object.Destroy(ingredient.gameObject);
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

        public void ReplaceRoot(int slotIndex, Pickup.Ingredient[] results)
        {
            Pickable content = _slots[slotIndex];
            if (content == null || content.Stack == null)
            {
                return;
            }

            _slots[slotIndex] = content.Stack.ReplaceRoot(results);
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
