using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Player
{
    [RequireComponent(typeof(Player_Movement))]
    public class Player_Jump : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float jumpForce;

        //Private/Hidden State parameters
        [HideInInspector] public bool playerHasJumped;
        [HideInInspector] public bool playerHasLanded;
        [HideInInspector] public bool playerIsFalling;
        private float airTimer;
        private float nextInputTimer;
        public float ceilingCheckLength;
        [SerializeField] private float nextInputTimerThreshold;
        [HideInInspector] public bool playerCanJump;
        [SerializeField] private LayerMask ignoreLayer;

        //Components
        Player_Movement playerMovement;
        PlayerInput playerInput;
        InputAction jumpAction;

        private void Awake()
        {
            playerMovement = GetComponent<Player_Movement>();
            playerInput = GetComponent<PlayerInput>();
            jumpAction = playerInput.actions["Jump"];
            ceilingCheckLength = jumpForce / Physics.gravity.magnitude;
        }

        void OnEnable() => playerMovement.JumpEvent += JumpEvent;
        void OnDisable() => playerMovement.JumpEvent -= JumpEvent;

        void JumpEvent()
        {
            var jumpInput = jumpAction.ReadValue<float>();
            if (jumpInput == 0) return;

            //Apply jump force if the player is not crouching and the player is grounded
            if (!playerMovement.isCrouching && playerMovement.characterController.isGrounded && playerCanJump)
            {
                playerHasJumped = true;
                playerCanJump = false;
                playerMovement.playerVelocity.y += jumpForce;
            }
        }

        void CeilingCheck()
        {
            if(Physics.Raycast(transform.position + Vector3.up, Vector3.up, out RaycastHit hit, ceilingCheckLength, ~ignoreLayer))
            {
                if(hit.collider != null)
                {
                    playerCanJump = false; //Gets reset to true following conditions below.
                }
            }
        }

        void LandDetection()
        {
            #region - Description -
            /*
             * When the player is no longer grounded the air timer increases.
             * Then there are two checks. The first determines if the player has landed by comparing
             * the isGrounded boolean by the air timer. If we are grounded on that frame and the airTimer
             * is greater than 0, we know that the player has freshly landed.
             * The second check (else if) ensures that the boolean playerHasLanded gets reset on the following frame.
             */
            #endregion

            if (!playerMovement.characterController.isGrounded)
            {
                airTimer += Time.deltaTime * 10f;
                playerIsFalling = true;
            }

            if (playerMovement.characterController.isGrounded && airTimer > 0)
            {
                airTimer = 0;
                playerHasJumped = false; //Reset the playerHasJumped value upon landing.
                playerIsFalling = false; //Reset the playerIsFalling value upon landing.
                playerHasLanded = true;
            }
            else if (playerMovement.characterController.isGrounded && airTimer == 0)
            {
                playerHasLanded = false;
            }
        }

        private void Update()
        {
            CeilingCheck();
            LandDetection();

            if(!playerCanJump)
            {
                nextInputTimer += Time.deltaTime;
            }
            //Once the timer hits it's threshold it gets reset to 0
            if (nextInputTimer >= nextInputTimerThreshold)
            {
                nextInputTimer = 0;
                playerCanJump = true;
            }
        }
    }
}