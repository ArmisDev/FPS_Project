using UnityEngine;
using System.Collections;
using Project.Player;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main), typeof(Animator))]
    public class Weapon_AnimationManager : MonoBehaviour
    {
        private Player_Movement player_Movement;
        private Player_Jump player_Jump;
        private Animator animator;
        private Weapon_Main weapon_Main;

        //Weapon Fire Animation
        private Transform fireAnimTransform; //Cannot use current transform since it is being used by animations.
        public AnimationCurve recoilCurvePosZ;
        public AnimationCurve recoilCurveRotY;
        [SerializeField] private float animationTime = 0.1f; // Duration of the animation
        private bool isAnimating = false;
        private Vector3 originalLocalPosition;
        private Quaternion originalLocalRotation;

        private void Awake()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            fireAnimTransform = GetComponentInParent<Transform>();
            animator = GetComponent<Animator>();
            weapon_Main = GetComponent<Weapon_Main>();
            
            if (playerObject != null)
            {
                player_Movement = playerObject.GetComponent<Player_Movement>();
                player_Jump = playerObject.GetComponent<Player_Jump>();
            }
            else
            {
                Debug.Log("player_Movement cannot be found, make sure the player is tagged as Player!!");
            }

            //Assign Dependents
            if (player_Movement == null || player_Jump == null)
            {
                Debug.LogWarning("Please attach Player_Movement & Player_Jump components to " + name + "!");
            }

            weapon_Main.OnFire += WeaponFireAnimation; // Part of Depricated Fire Animation

            // The original local position and rotation
            originalLocalPosition = Vector3.zero;
            originalLocalRotation = new Quaternion(0, 0, 0, 0);
            originalLocalPosition = fireAnimTransform.localPosition;
            originalLocalRotation = fireAnimTransform.localRotation;

        }

        private void OnDestroy()
        {
            weapon_Main.OnFire -= WeaponFireAnimation; // Part of Depricated Fire Animation
        }

        void LocomotionAnimation()
        {
            //Grab the magnitude of the XZ cordinates from the playervelocity vector.
            //Doing this ensures that the weapon animation for locomotion is not affected by the Y axis.
            Vector2 playerVelocityXZ = new Vector2(player_Movement.playerVelocity.x, player_Movement.playerVelocity.z);
            var newPlayerVelocity = playerVelocityXZ.magnitude;
            if(newPlayerVelocity < 0.05) //If the value is under this threshold just set the value to zero.
            {
                newPlayerVelocity = 0f;
            }
            animator.SetFloat("LocomotionBlend", newPlayerVelocity);
        }

        void JumpAnimation()
        {
            animator.SetBool("PlayerCanJump", player_Jump.playerCanJump);
            animator.SetBool("PlayerHasJumped", player_Jump.playerHasJumped);
            animator.SetBool("PlayerHasLanded", player_Jump.playerHasLanded);
            animator.SetBool("PlayerIsFalling", player_Jump.playerIsFalling);
        }

        void CrouchAnimation()
        {
            animator.SetBool("PlayerIsCrouching", player_Movement.isCrouching);
        }

        void ReloadAnimation()
        {
            animator.SetBool("Reloading", weapon_Main.weaponIsReloading);
        }

        #region - Procedural Fire Animation -
        //!!Event based. Called via Weapon_Main Fire Method!!
        void WeaponFireAnimation()
        {
            if (weapon_Main.currentFireMode == Weapon_Main.WeaponFireModes.automatic)
            {
                if (!isAnimating)
                {
                    StartCoroutine(FireAnimation());
                }
            }
            else if (weapon_Main.currentFireMode == Weapon_Main.WeaponFireModes.semiautomatic)
            {
                StopCoroutine(FireAnimation()); // Stop if already running
                StartCoroutine(FireAnimation()); // Start a new instance
            }
        }

        private IEnumerator FireAnimation()
        {
            isAnimating = true;
            float time = 0;

            while (time < animationTime)
            {
                time += Time.deltaTime;
                float curveValuePosZ = recoilCurvePosZ.Evaluate(time / animationTime);
                float curveValueRotY = recoilCurveRotY.Evaluate(time / animationTime);

                // Apply animation changes
                fireAnimTransform.localPosition += new Vector3(0, 0, curveValuePosZ);
                fireAnimTransform.localEulerAngles += new Vector3(0, curveValueRotY, 0);

                yield return null;
            }

            isAnimating = false;
            ResetToOriginalTransform(); // Reset at the end of the coroutine
        }

        public void ResetToOriginalTransform()
        {
            fireAnimTransform.localPosition = originalLocalPosition;
            fireAnimTransform.localRotation = originalLocalRotation;
        }
        #endregion

        void FireAnimationTrigger()
        {
            animator.SetBool("WeaponIsAuto", weapon_Main.currentFireMode == Weapon_Main.WeaponFireModes.automatic);
            animator.SetBool("WeaponIsFired", weapon_Main.weaponIsFiring);
            animator.SetBool("SemiAutoHasInput", weapon_Main.newFireSemi > 0);
            animator.SetFloat("AmmoCount", weapon_Main.ammoCount);
        }

        private void Update()
        {
            //Debug.Log(isAnimating);
            FireAnimationTrigger();
            ReloadAnimation();
            //if (!isAnimating)
            //{
            //    ResetToOriginalTransform();
            //}
            if (player_Movement.characterController.isGrounded)
            {
                LocomotionAnimation();
            }

            JumpAnimation();
            CrouchAnimation();
        }
    }
}