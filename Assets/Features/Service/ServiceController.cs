using UnityEngine;

namespace Features.Service
{
    public class ServiceController : MonoBehaviour
    {
        void Start()
        {
            Core.CursorController.Lock();
        }
    }
}
