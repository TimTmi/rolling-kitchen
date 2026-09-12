using UnityEngine;

namespace Features.Service
{
    public class ServiceController : MonoBehaviour
    {
        [SerializeField] private CrosshairController crosshairController;
    
        void Start()
        {
            Core.CursorController.Lock();
            crosshairController.Show();
            
            Debug.Log($"Cursor: {Cursor.lockState}, visible: {Cursor.visible}");
        }
    }
}
