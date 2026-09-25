using Features.Interaction;
using UnityEngine;

namespace Features.Dish
{
    public class OrderSlot : MonoBehaviour
    {
        [SerializeField] private Tray tray;
        [SerializeField] private Transform pathStart;
        [SerializeField] private Transform pathEnd;

        public Tray Tray => tray;
        public Transform PathStart => pathStart;
        public Transform PathEnd => pathEnd;
    }
}
