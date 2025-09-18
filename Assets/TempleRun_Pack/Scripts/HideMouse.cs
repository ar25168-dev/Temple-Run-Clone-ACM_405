using UnityEngine;

public class HideMouse : MonoBehaviour
{
    private void Start()
    {
        // Hide cursor and lock it to the center
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        // Restore cursor when this script is disabled (optional)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
