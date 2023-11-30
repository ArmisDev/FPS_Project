using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Player
{
    public class Player_Crouch : MonoBehaviour
    {
        //[Header("Parameters")]
        //[SerializeField] private float crouchHeight;
        //private float crouchSpeed;
        //private float initCharacterControllerHeight;
        //private bool cantStand;
        //public float ceilingCheckLength;
        //private float crouchTimer;
        //[SerializeField] private LayerMask ignoreLayer;

        ////Components
        //Player_Movement playerMovement;
        //PlayerInput playerInput;
        //InputAction crouchAction;

        //enum PlayerState
        //{
        //    Standing,
        //    Crouching,
        //    AttemptingToStand
        //}

        //PlayerState currentState = PlayerState.Standing;


        //private void Awake()
        //{
        //    playerMovement = GetComponent<Player_Movement>();
        //    playerInput = GetComponent<PlayerInput>();
        //    crouchAction = playerInput.actions["Crouch"];
        //}

        //private void Start()
        //{
        //    initCharacterControllerHeight = playerMovement.characterController.height;
        //    ceilingCheckLength = playerMovement.characterController.height / 2f;
        //}

        ////void OnEnable() => playerMovement.CrouchEvent += CrouchEvent;
        ////void OnDisable() => playerMovement.CrouchEvent -= CrouchEvent;

        //void CeilingCheck()
        //{
        //    cantStand = Physics.Raycast(transform.position + Vector3.up, Vector3.up, ceilingCheckLength, ~ignoreLayer);
        //    Debug.Log(cantStand);
        //}

        //void UpdatePlayerState()
        //{
        //    // Check if the player is trying to stand
        //    if (currentState == PlayerState.Crouching && crouchAction.ReadValue<float>() == 0)
        //    {
        //        currentState = PlayerState.AttemptingToStand;
        //    }

        //    // If the player is attempting to stand but can't due to an obstacle
        //    if (currentState == PlayerState.AttemptingToStand && cantStand)
        //    {
        //        currentState = PlayerState.Crouching; // Remain crouched
        //    }

        //    // If the player can stand (no obstacle above), transition to standing
        //    if (currentState == PlayerState.AttemptingToStand && !cantStand)
        //    {
        //        StandUp();
        //        currentState = PlayerState.Standing;
        //    }

        //    // Check if the player is trying to crouch
        //    if (currentState == PlayerState.Standing && crouchAction.ReadValue<float>() != 0)
        //    {
        //        Crouch();
        //        currentState = PlayerState.Crouching;
        //    }
        //}

        //void Crouch()
        //{
        //    playerMovement.isCrouching = true;
        //    playerMovement.characterController.height = crouchHeight;
        //    // Additional crouching logic here
        //}

        //void StandUp()
        //{
        //    playerMovement.isCrouching = false;
        //    playerMovement.characterController.height = initCharacterControllerHeight;
        //    // Additional stand-up logic here
        //}


        ////void CrouchEvent()
        ////{
        ////    #region - Description -
        ////    /*
        ////     * First we grab the input value for crouching
        ////     * Then we make sure to set our CharacterController height to it's initial value and return if there is no input.
        ////     * Next, if there is input we set the isCrouch bool to true.
        ////     * Then we set the speedadjuster to be a value of 0.5. This makes sure that are default speed is halved when crouching.
        ////     * Then the height of the player gets adjusted
        ////     * Finally, we apply the crouch speed to the player.
        ////     */
        ////    #endregion
        ////    var crouchInput = crouchAction.ReadValue<float>();
            
        ////    if (crouchInput == 0 && !playerMovement.cantStand)
        ////    {
        ////        playerMovement.isCrouching = false;
        ////        playerMovement.characterController.height = initCharacterControllerHeight;
        ////        return;
        ////    }

        ////    //Crouch and run cannot not be performed together.
        ////    playerMovement.isRunning = false;
        ////    playerMovement.crouchHeight = crouchHeight;
        ////    playerMovement.isCrouching = true;
        ////    crouchSpeed = playerMovement.speedAdjuster = 0.5f;

        ////    playerMovement.heightChanged = false; //This is set to false so that are movement class can adjust the player height back when standing back up.
        ////    playerMovement.characterController.height = crouchHeight;
        ////    playerMovement.speedAdjuster = crouchSpeed;
        ////}

        //private void Update()
        //{
        //    CeilingCheck();
        //    UpdatePlayerState();
        //}

    }
}
