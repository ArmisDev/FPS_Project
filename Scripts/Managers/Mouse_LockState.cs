using UnityEngine;

public class Mouse_LockState : MonoBehaviour
{
    // Enum to define control priority
    public enum ControlPriority { None, Popup, PauseMenu }

    public bool cursorVisibility = false;
    private CursorLockMode cursorLockMode = CursorLockMode.Locked;
    private ControlPriority currentPriority = ControlPriority.None;

    private void Start()
    {
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
    }

    public void RequestControl(ControlPriority priority, bool visibility, CursorLockMode mode)
    {
        if (priority >= currentPriority)
        {
            // Set new control priority
            currentPriority = priority;

            // Update cursor state
            cursorVisibility = visibility;
            cursorLockMode = mode;
            UpdateCursorState();
        }
    }

    public void ReleaseControl(ControlPriority priority)
    {
        // Only release control if the priority matches
        if (currentPriority == priority)
        {
            currentPriority = ControlPriority.None;
        }
    }
}