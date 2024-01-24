using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Project.Player;
using static UnityEngine.InputSystem.InputAction;
using System.Numerics;

namespace Project.ProjectDebug
{
    public class Debug_DeveloperConsoleBehavior : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private Debug_ConsoleCommand[] commands = new Debug_ConsoleCommand[0];
        [SerializeField] private Player_Movement player;
        [SerializeField] private Mouse_LockState mouseLockState;

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas = null;
        [SerializeField] private TMP_InputField inputField = null;

        private float pausedTimeScale;

        private static Debug_DeveloperConsoleBehavior instance;

        private Debug_DeveloperConsole developerConsole;

        private Debug_DeveloperConsole DeveloperConsole
        {
            get
            {
                if(developerConsole != null) { return developerConsole; }
                return developerConsole = new Debug_DeveloperConsole (prefix, commands);
            }
        }

        private void Awake()
        {
            if(instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ToggleDebug(CallbackContext context)
        {
            if (!context.action.triggered) { return; }

            if (uiCanvas.activeSelf)
            {
                player.stopLookRotation = false;
                mouseLockState.RequestControl(Mouse_LockState.ControlPriority.None, false, CursorLockMode.Locked);
                mouseLockState.ReleaseControl(Mouse_LockState.ControlPriority.None);
                Time.timeScale = pausedTimeScale;
                uiCanvas.SetActive(false);
            }
            else
            {
                player.stopLookRotation = true;
                pausedTimeScale = Time.timeScale;
                mouseLockState.RequestControl(Mouse_LockState.ControlPriority.None, true, CursorLockMode.Confined);
                Time.timeScale = 0;
                uiCanvas.SetActive(true);
                inputField.ActivateInputField();
            }
        }

        public void ProcessCommand(string inputValue)
        {
            DeveloperConsole.ProcessCommand(inputValue);
            inputField.text = string.Empty;
        }
    }
}
