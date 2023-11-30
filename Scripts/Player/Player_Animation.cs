using UnityEngine;

namespace Project.Player
{
    [RequireComponent(typeof(Player_Movement))]
    public class Player_Animation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Animator animator;
        private Player_Movement player_Movement;

        [Header("Sprint FOV Parameters")]
        [SerializeField] private float targetFOV;
        [SerializeField] private float fovAccelerate;
        private float initalFOV;
        private float resetTheshold = 80.05f; //Used in the Approximatley Method to say any value below are reset threshold
                                     //Is the same as our initialFOV.

        private void Awake()
        {
            initalFOV = playerCamera.fieldOfView;
            player_Movement = GetComponent<Player_Movement>();
        }

        private void HeadBob()
        {
            //This adjusts the blend float on the locomotion blend tree to match the magnitude of the players velocity.
            animator.SetFloat("LocomotionBlend", player_Movement.playerVelocity.magnitude);
        }

        private void CameraFOVChange()
        {
            var accelFactor = fovAccelerate * Time.deltaTime; //float to lerp with
            var currentFOV = playerCamera.fieldOfView;

            if (!player_Movement.isRunning && playerCamera.fieldOfView == initalFOV) return;

            //If we are running and not at the target FOV, then adjust the FOV
            if (player_Movement.isRunning && playerCamera.fieldOfView <= targetFOV)
            {
                playerCamera.fieldOfView = Mathf.Lerp(currentFOV, targetFOV, accelFactor);
            }
            //If we arent runing and the FOV is greater or equal to the target FOV, then lerp back.
            else if (!player_Movement.isRunning)
            {
                playerCamera.fieldOfView = Mathf.Lerp(currentFOV, initalFOV, accelFactor);
            }
        }

        private void Update()
        {
            if(playerCamera.fieldOfView <= resetTheshold)
            {
                playerCamera.fieldOfView = initalFOV;
            }

            HeadBob();
            CameraFOVChange();
        }
    }
}