using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Player
{
    public class Player_NewCrouch : MonoBehaviour
    {
        [Header("Crouch Parameters")]
        [SerializeField] private float crouchHeight;
        [SerializeField] private float ceilingCheckLength;
        [SerializeField] private LayerMask ignoreLayer;
        [SerializeField] private float crouchSpeed = 0.5f;
        private Player_Movement playerMovement;
        private InputAction crouchAction;

        [HideInInspector] public float standingHeight; // Variable to store the standing height
        [HideInInspector] public bool isCrouching;
        private bool heightChanged;

        //Slide
        [SerializeField] private bool canSlide;
        [SerializeField] private float slideTime;
        public float slideTimeReference;

        private void Awake()
        {
            playerMovement = GetComponent<Player_Movement>();
            var playerInput = GetComponent<PlayerInput>();
            crouchAction = playerInput.actions["Crouch"];
        }

        private void Start()
        {
            standingHeight = playerMovement.characterController.height; // Initialize standingHeight
            ceilingCheckLength = standingHeight / 2f;
        }

        private bool IsCeilingBlocking()
        {
            // Check for ceiling
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out RaycastHit hit, ceilingCheckLength, ~ignoreLayer))
            {
                return hit.collider != null; // Returns true if there's a collider, false otherwise
            }
            return false; // Returns false if the raycast did not hit anything
        }

        private void Update()
        {
            HandleCrouchInput();
            HandleCrouchState();

            //Ensures that are player does not clip through ground when standing back up
            //Works by taking the differnce between are stand height and crouch height and multiplies this value by 0.5
            //The result is that the player gets moved above the ground when standing back up.
            if (!heightChanged && !isCrouching && !IsCeilingBlocking())
            {
                playerMovement.characterController.Move(Vector3.up * (standingHeight - crouchHeight) * 0.5f);
                heightChanged = true;
            }
        }

        private void HandleCrouchInput()
        {
            // Toggle crouch state based on input
            if (crouchAction.triggered)
            {
                isCrouching = !isCrouching;
            }
        }

        private void HandleCrouchState()
        {
            if (isCrouching && !IsCeilingBlocking())
            {
                // If player is crouching and there is no ceiling blocking, continue crouching
                Crouch();
            }

            // If player is crouching and there is a ceiling blocking, force crouch
            if (isCrouching && IsCeilingBlocking())
            {
                ForceCrouch();
            }

            else if (!isCrouching && !IsCeilingBlocking())
            {
                // If player is not crouching, stand up
                StandUp();
            }

            //else if (isCrouching && playerMovement.playerVelocity.magnitude > playerMovement.defaultSpeed && canSlide)
            //{
            //    slideTimeReference = slideTime;
            //    StartCoroutine(SlideCoroutine());
            //}
        }

        private void Crouch()
        {
            playerMovement.characterController.height = crouchHeight;
            
            heightChanged = false;
            playerMovement.isCrouching = true;
            playerMovement.speedAdjuster = crouchSpeed;
        }

        //private IEnumerator SlideCoroutine()
        //{
        //    playerMovement.characterController.height = crouchHeight;
        //    heightChanged = false;
        //    playerMovement.isCrouching = true;

        //    float slideDuration = 1.0f; // Adjust this value to control the slide duration
        //    float slideTimer = slideDuration;

        //    while (slideTimer > 0)
        //    {
        //        Debug.Log("Slide Test");

        //        // Calculate the slide time ratio
        //        float slideTimeRatio = 1.0f - (slideTimer / slideDuration);

        //        // Use Mathf.Lerp to interpolate the speedAdjuster value
        //        playerMovement.speedAdjuster = Mathf.Lerp(playerMovement.speedAdjuster, crouchSpeed, slideTimeRatio);

        //        // Decrement the timer
        //        slideTimer -= Time.deltaTime;

        //        // Ensure the slideTimer does not go negative
        //        if (slideTimer < 0)
        //        {
        //            slideTimer = 0;
        //        }

        //        // Yield control back to Unity for one frame
        //        yield return null;
        //    }

        //    // Ensure the final value is set to crouchSpeed
        //    playerMovement.speedAdjuster = crouchSpeed;

        //    // Reset isCrouching if needed
        //    playerMovement.isCrouching = false;
        //}

        private void StandUp()
        {
            playerMovement.characterController.height = standingHeight;
            playerMovement.isCrouching = false;
        }

        private void ForceCrouch()
        {
            playerMovement.characterController.height = crouchHeight;
        }

    }
}
