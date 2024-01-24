using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player_Movement : MonoBehaviour
    {
        #region - Components -
        [Header("Components")]
        [SerializeField] private Transform cameraHolderTransform;

        //Private Components
        [HideInInspector] public CharacterController characterController;
        #endregion

        #region - Parameters -
        [Header("Movement Parameters")]
        [SerializeField] private float playerMass;
        public float defaultSpeed;
        [SerializeField] private float acceleration; //acceleration controls the time it takes for player to reach full velocity
        public bool stopMovement;
        public bool stopLookRotation;

        [Header("Look Parameters")]
        [SerializeField] private float lookSensitivity;
        [SerializeField] private bool useMouseAcceleration;
        [SerializeField] private float lookAcceleration;

        //Private/Hidden Movement Parameters
        [HideInInspector] public Vector3 playerVelocity;
        [HideInInspector] public float speedAdjuster;

        //Private Look Parameters
        private float lookInputX;
        private float lookInputY;
        private Vector3 lookRotation;
        #endregion

        #region - Input -
        private PlayerInput playerInput;
        private InputAction input_Move;
        private InputAction input_Look;
        private InputAction input_Jump;
        private InputAction input_Sprint;
        #endregion

        #region - State -

        //Bool States
        public bool isCrouching;
        [HideInInspector] public bool isRunning;

        //Character Height
        private float characterHeight;
        [HideInInspector] public float crouchHeight; //Value set by Player_Crouch

        //All movment states go here (ENSURE WALKING IS FIRST!)
        public enum State
        {
            walking,
            climbing,
            swimming,
            tutorial
        }

        //Variable we can call to access state
        State state;

        // Current state allows you to get information on the current state
        // Or, you can set the current state to a new state
        public State currentState
        {
            get => state;
            set
            {
                state = value;
                playerVelocity = Vector3.zero;
            }
        }
        #region - New StateChange Attempt (Dedeprecated)
        //public State _currentState;
        //public State _previousState;
        //Most Recent State
        //[HideInInspector] public State previousState;
        //public State currentState
        //{
        //    get => _currentState;
        //    private set => _currentState = value;
        //}

        //public State previousState
        //{
        //    get => _previousState;
        //    private set => _previousState = value;
        //}

        //// Call this method to change the state
        //public void ChangeState(State newState)
        //{
        //    if (_currentState != newState)
        //    {
        //        _previousState = _currentState;
        //        _currentState = newState;
        //        playerVelocity = Vector3.zero;
        //    }
        //}
        #endregion

        [HideInInspector] public bool bypassInitialState;
        #endregion

        #region - Events
        public event Action RunEvent;
        public event Action JumpEvent;
        public event Action CrouchEvent;
        #endregion

        private void Awake()
        {
            stopMovement = false;
            stopLookRotation = false;

            if(!bypassInitialState)
            {
                currentState = State.walking;
            }

            characterController = GetComponentInParent<CharacterController>(); //Grabs Character Controller
            characterHeight = characterController.height;

            //Get Input System & InputActions
            playerInput = GetComponent<PlayerInput>();
            input_Move = playerInput.actions["Move"];
            input_Look = playerInput.actions["Look"];
            input_Jump = playerInput.actions["Jump"];
            input_Sprint = playerInput.actions["Sprint"];
        }

        #region - Movement -
        private Vector3 GetMoveInput(float defaultSpeed, bool useHorizontalMovement)
        {
            #region  - Desciption -
            /* 
             * First we check to see if there are any events active, and if so, we perform them.
             * Then we are grabing the move input from the input system
             * Then we assign a blank Vector3
             * Followed by grabbing the transform we wish to move along (This determines direction).
             * If useHorizontalMovement is true we use the cameraHolderTransform, else, we use the gameobject transform.
             * Then we subscribe the blank Vector3 (ie. Input) to the transforms forward Vector * by the forward movement input
             * We then repeat that logic for the horizontal direction
             * Once our forward and side directions our recieving input and transforms we want to now add a speed value and a run value.
             * Run value can be 1 if not running our anything above if we are running
             * Finally we return the input Vector
             */
            #endregion
            
            CrouchEvent?.Invoke();
            RunEvent?.Invoke();
            JumpEvent?.Invoke();

            var moveInput = input_Move.ReadValue<Vector2>();
            var input = new Vector3();
            var referenceTransform = useHorizontalMovement ? transform : cameraHolderTransform;
            input += referenceTransform.forward * moveInput.y;
            input += referenceTransform.right * moveInput.x;
            input *= defaultSpeed * speedAdjuster;
            return input;
        }

        private void DefaultMovementLogic()
        {
            #region  - Desciption -
            /* 
             * By default runSpeedMult will equal zero. This is so if we are not running then defaultSpeed gets multiplied against itself.
             * Next, we want to call the GetMovementInput() and have the local input variable equal the input from the GetMoveInput().
             * Then, accelFactor is created to be a float value that we can use to lerp between our current velocity (referenced as playerVelocity) and input (essentially max velocity).
             * After accelFactor is created we then want to lerp between our current velocity (referenced as playerVelocity) and input (essentially max velocity).
             * Finally, we use move the characterController by using our playerVelocity Vector.
             */
            #endregion

            if(!isCrouching || !isRunning)
            {
                speedAdjuster = 1;
            }

            var input = GetMoveInput(defaultSpeed, true);
            var accelFactor = acceleration * Time.deltaTime;
            playerVelocity.x = Mathf.Lerp(playerVelocity.x, input.x, accelFactor);
            playerVelocity.z = Mathf.Lerp(playerVelocity.z, input.z, accelFactor);

            characterController.Move((playerVelocity) * Time.deltaTime);
        }

        void UpdateGravity()
        {
            var gravity = Physics.gravity * playerMass * Time.deltaTime;
            playerVelocity.y = characterController.isGrounded ? -1f : playerVelocity.y + gravity.y;
        }
        #endregion

        #region - Look -
        void LookLogic()
        {
            #region - Description
            /*
             * First we want to grab the input information from our Input System
             * Then the input needs to be seperated into X & Y and then multiplied by the lookSensivity value.
             * Next, the y rotation needs to be clamped so the rotation can't rotate 360 degrees.
             * Then we rotate the camera holder by the inverted Y input and rotate the player by the X input.
             */
            #endregion
            var lookInput = input_Look.ReadValue<Vector2>();

            lookInputX += lookInput.x * lookSensitivity;
            lookInputY += lookInput.y * lookSensitivity;

            lookInputY = Mathf.Clamp(lookInputY, -89, 89);

            if (useMouseAcceleration)
            {
                var lookAccelFactor = lookAcceleration * Time.deltaTime;
                lookRotation.x = Mathf.Lerp(lookRotation.x, lookInputX, lookAccelFactor);
                lookRotation.y = Mathf.Lerp(lookRotation.y, lookInputY, lookAccelFactor);

                cameraHolderTransform.localRotation = Quaternion.Euler(-lookRotation.y, 0, 0);
                transform.localRotation = Quaternion.Euler(0, lookRotation.x, 0);
            }

            else
            {
                cameraHolderTransform.localRotation = Quaternion.Euler(-lookInputY, 0, 0);
                transform.localRotation = Quaternion.Euler(0, lookInputX, 0);
            }
        }
        #endregion

        private void Update()
        {
            if(!stopLookRotation)
            {
                LookLogic();
            }
        }

        private void FixedUpdate()
        {
            //If we can move, we should move
            if(!stopMovement)
            {
                switch (currentState)
                {
                    //Most if these states perform the same logic, but are still seperated due to external systems adjusting themselves based of different player states
                    //Ex: The Weapon_AnimationManager is dependent on this class and certian animations play based on the players state.
                    case State.walking:
                        UpdateGravity();
                        DefaultMovementLogic();
                        break;
                }
            }
        }
    }
}