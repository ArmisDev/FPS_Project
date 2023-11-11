using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Player
{
    [RequireComponent(typeof(Player_Movement))]
    public class Player_Sprint : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float runMultiplier;

        //Components
        Player_Movement playerMovement;
        PlayerInput playerInput;
        InputAction sprintAction;

        private void Awake()
        {
            playerMovement = GetComponent<Player_Movement>();
            playerInput = GetComponent<PlayerInput>();
            sprintAction = playerInput.actions["Sprint"];
        }

        void OnEnable() => playerMovement.RunEvent += RunEvent;
        void OnDisable() => playerMovement.RunEvent -= RunEvent;

        void RunEvent()
        {
            var sprintInput = sprintAction.ReadValue<float>();
            if(sprintInput == 0)
            {
                playerMovement.isRunning = false;
                return;
            }

            #region - Description -
            /*
             * First we set playerrunning to true and then we create a vector3 dot product that combines the forward direction of the player and the players velocity.
             * By doing this, we can ensure that the player speed decreases as the player rotates away from the forward direction.
             * Then, we lerp from a base value of 1 to our runMuliplier value forwardMoveFactor float.
             * Finally, we multiply our speedAdjuster by the multiplier value.
             */
            #endregion
            playerMovement.isRunning = true;
            var forwardMoveFactor = Mathf.Clamp01(Vector3.Dot(playerMovement.transform.forward, playerMovement.playerVelocity.normalized));
            var multiplier = Mathf.Lerp(1f, runMultiplier, forwardMoveFactor);
            playerMovement.speedAdjuster *= multiplier;
        }
    }
}
