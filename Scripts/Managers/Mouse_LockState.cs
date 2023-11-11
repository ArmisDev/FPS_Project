using UnityEngine;

public class Mouse_LockState : MonoBehaviour
{
    void Start()
    {
        // Lock the cursor to the center of the game window
        Cursor.lockState = CursorLockMode.Locked;

        // Hide the cursor from view
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Press ESC to unlock and show the cursor
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            Debug.Log("Test");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Press F to re-lock and hide the cursor
        if (Input.GetKeyDown(KeyCode.F))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
