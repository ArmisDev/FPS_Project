using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Player
{
    public class Player_Crouch : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float crouchHeight;
        private float crouchSpeed;
        private float initCharacterControllerHeight;

        //Components
        Player_Movement playerMovement;
        PlayerInput playerInput;
        InputAction crouchAction;

        private void Awake()
        {
            playerMovement = GetComponent<Player_Movement>();
            playerInput = GetComponent<PlayerInput>();
            crouchAction = playerInput.actions["Crouch"];
        }

        private void Start()
        {
            initCharacterControllerHeight = playerMovement.characterController.height;
        }

        void OnEnable() => playerMovement.CrouchEvent += CrouchEvent;
        void OnDisable() => playerMovement.CrouchEvent -= CrouchEvent;

        void CrouchEvent()
        {
            #region - Description -
            /*
             * First we grab the input value for crouching
             * Then we make sure to set our CharacterController height to it's initial value and return if there is no input.
             * Next, if there is input we set the isCrouch bool to true.
             * Then we set the speedadjuster to be a value of 0.5. This makes sure that are default speed is halved when crouching.
             * Then the height of the player gets adjusted
             * Finally, we apply the crouch speed to the player.
             */
            #endregion
            var crouchInput = crouchAction.ReadValue<float>();
            if (crouchInput == 0)
            {
                playerMovement.isCrouching = false;
                playerMovement.characterController.height = initCharacterControllerHeight;
                return;
            }

            //Crouch and run cannot not be performed together.
            playerMovement.isRunning = false;

            playerMovement.isCrouching = true;
            crouchSpeed = playerMovement.speedAdjuster = 0.5f;
            playerMovement.characterController.height = crouchHeight;
            playerMovement.speedAdjuster = crouchSpeed;
        }
    }
}
