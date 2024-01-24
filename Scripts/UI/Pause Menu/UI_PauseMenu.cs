using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Project.Player;

namespace Project.UI
{
    [RequireComponent(typeof(PlayerInput))]
    public class UI_PauseMenu : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject NonPauseObjectHolder;
        [SerializeField] private GameObject pauseMenuGameObject;
        [SerializeField] private GameObject exitMenuOptions;
        [SerializeField] private Mouse_LockState mouseLockState;
        [SerializeField] private Player_Movement movement;

        private bool hideNonPause;
        private bool pauseIsActive;

        //Input
        private PlayerInput input;
        private InputAction menuToggle;

        private event Action OnPauseToggle;

        private void Awake()
        {
            //Caching
            input = GetComponent<PlayerInput>();
            menuToggle = input.actions["MenuToggle"];

            if (NonPauseObjectHolder == null || pauseMenuGameObject == null)
            {
                Debug.LogError("Please attach NonPauseObjectHolder to UI_PauseMenu!!");
                return;
            }

            //ensure the pause menu starts as non active
            pauseMenuGameObject.SetActive(false);
        }

        void Update()
        {
            //Set hidenonpuase to equal the opposite of if our pause menu is active.
            hideNonPause = !pauseMenuGameObject.activeSelf;

            if(menuToggle.WasPerformedThisFrame())
            {
                PauseMenu();
            }

            NonPauseObjectHolder.SetActive(hideNonPause);
        }

        void PauseMenu()
        {
            if(!pauseMenuGameObject.activeSelf)
            {
                Time.timeScale = 0;
                pauseMenuGameObject.SetActive(true);
                mouseLockState.RequestControl(Mouse_LockState.ControlPriority.PauseMenu, true, CursorLockMode.Confined);
                movement.stopLookRotation = true;
            }
            else
            {
                Time.timeScale = 1;
                pauseMenuGameObject.SetActive(false);
                mouseLockState.RequestControl(Mouse_LockState.ControlPriority.PauseMenu, false, CursorLockMode.Locked);
                mouseLockState.ReleaseControl(Mouse_LockState.ControlPriority.PauseMenu);
                movement.stopLookRotation = false;
            }
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            pauseMenuGameObject.SetActive(false);
            mouseLockState.RequestControl(Mouse_LockState.ControlPriority.PauseMenu, false, CursorLockMode.Locked);
            mouseLockState.ReleaseControl(Mouse_LockState.ControlPriority.PauseMenu);
            movement.stopLookRotation = false;
        }

        public void OnNewGameClicked()
        {
            DataPersistanceManager.instance.NewGame();
        }

        public void OnSavedGameClicked()
        {
            DataPersistanceManager.instance.SaveGame();
        }

        public void OnLoadGameClicked()
        {
            DataPersistanceManager.instance.LoadGame();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}