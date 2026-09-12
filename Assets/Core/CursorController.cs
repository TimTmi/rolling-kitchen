using UnityEngine;

namespace Core
{
    public static class CursorController
    {
        public static void Lock()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void Unlock()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

