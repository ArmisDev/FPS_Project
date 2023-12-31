using UnityEngine;
using System.Collections;
using Project.Player;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main), typeof(Animator), typeof(AudioSource))]
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

        //Falling related variables
        public float fallTime;
        float fallThreshold = 0.2f;

        //Shotgun Unique
        private AudioSource audioSource;
        [SerializeField] private AudioClip loadShellClip;
        [SerializeField] private AudioClip pumpRound;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
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
            //Falling is handled in a different method due to there being different ways in which a player can fall
            //This means that it requires more specification
        }

        void FallingAnimation()
        {
            switch(player_Movement.characterController.isGrounded)
            {
                case true:
                    fallTime = 0;
                    break;
                case false:
                    fallTime += Time.deltaTime;
                    break;
            }

            //In animator there is a constraint that we cant go directly falling animation if we jumped.
            //Doing it this way ensures that when we jump the falling animation will play as normal, but if
            //we are just falling then we must excede our threshold.
            if (fallTime > fallThreshold || player_Jump.playerHasJumped)
            {
                animator.SetBool("PlayerIsFalling", player_Jump.playerIsFalling);
            }
            else
            {
                animator.SetBool("PlayerIsFalling", false);
            }
        }

        void CrouchAnimation()
        {
            animator.SetBool("PlayerIsCrouching", player_Movement.isCrouching);
        }


        //Ass logic for shotgun
        void ReloadAnimation()
        {
            animator.SetBool("Reloading", weapon_Main.weaponIsReloading);
        }

        //Called via Animation Event
        void PlayPumpRoundClip()
        {
            PlayAudio(pumpRound);
        }

        //Called via Animation Event
        void PlayLoadRoundClip()
        {
            PlayAudio(loadShellClip);
        }

        void PlayAudio(AudioClip audioClip)
        {
            audioSource.PlayOneShot(audioClip);
        }

        //Called via Animation Event
        void AddShellForShotgun()
        {
            weapon_Main.ammoCount++;
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
            //switch(weapon_Main.currentFireMode)
            //{
            //    case Weapon_Main.WeaponFireModes.automatic:
            //        animator.SetBool("WeaponIsAuto", weapon_Main.currentFireMode == Weapon_Main.WeaponFireModes.automatic);
            //        animator.SetBool("WeaponIsFired", weapon_Main.weaponIsFiring);
            //        animator.SetInteger("AmmoCount", weapon_Main.ammoCount);
            //        break;
            //    case Weapon_Main.WeaponFireModes.semiautomatic:
            //        animator.SetBool("SemiAutoHasInput", weapon_Main.newFireSemi > 0);
            //        animator.SetBool("WeaponIsFired", weapon_Main.weaponIsFiring);
            //        animator.SetInteger("AmmoCount", weapon_Main.ammoCount);
            //        break;
            //    case Weapon_Main.WeaponFireModes.shotgun:
            //        animator.SetBool("SemiAutoHasInput", weapon_Main.newFireSemi > 0);
            //        animator.SetFloat("timeSinceFire", weapon_Main.timeSinceFire);
            //        animator.SetBool("CanFire", weapon_Main.allowedToFire);
            //        animator.SetBool("WeaponIsFired", weapon_Main.weaponIsFiring);
            //        animator.SetInteger("AmmoCount", weapon_Main.ammoCount);
            //        break;

            //}
            //if(weapon_Main.currentFireMode == Weapon_Main.WeaponFireModes.shotgun)
            //{
            //    animator.SetBool("CanFire", weapon_Main.allowedToFire);
            //}
            //animator.SetBool("WeaponIsAuto", weapon_Main.currentFireMode == Weapon_Main.WeaponFireModes.automatic);
            //animator.SetBool("WeaponIsFired", weapon_Main.weaponIsFiring);
            //animator.SetBool("SemiAutoHasInput", weapon_Main.newFireSemi > 0);
            //animator.SetInteger("AmmoCount", weapon_Main.ammoCount);
        }

        private void Update()
        {
            switch (weapon_Main.currentFireMode)
            {
                case Weapon_Main.WeaponFireModes.automatic:
                    animator.SetBool("WeaponIsAuto", weapon_Main.currentFireMode == Weapon_Main.WeaponFireModes.automatic);
                    animator.SetBool("WeaponIsFired", weapon_Main.weaponIsFiring);
                    animator.SetFloat("timeSinceFire", weapon_Main.timeSinceFire);
                    animator.SetInteger("AmmoCount", weapon_Main.ammoCount);
                    break;
                case Weapon_Main.WeaponFireModes.semiautomatic:
                    animator.SetBool("SemiAutoHasInput", weapon_Main.newFireSemi > 0);
                    animator.SetFloat("timeSinceFire", weapon_Main.timeSinceFire);
                    animator.SetBool("WeaponIsFired", weapon_Main.weaponIsFiring);
                    animator.SetInteger("AmmoCount", weapon_Main.ammoCount);
                    break;
                case Weapon_Main.WeaponFireModes.shotgun:
                    if (weapon_Main.ammoCount == weapon_Main.maxAmmo - 1)
                    {
                        animator.SetBool("ReachedMaxAmmo", true);
                    }
                    else animator.SetBool("ReachedMaxAmmo", false);

                    animator.SetBool("SemiAutoHasInput", weapon_Main.newFireSemi > 0);
                    animator.SetFloat("timeSinceFire", weapon_Main.timeSinceFire);
                    animator.SetBool("CanFire", weapon_Main.allowedToFire);
                    animator.SetBool("WeaponIsFired", weapon_Main.weaponIsFiring);
                    animator.SetInteger("AmmoCount", weapon_Main.ammoCount);
                    break;

            }
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
            FallingAnimation();
            CrouchAnimation();
        }
    }
}