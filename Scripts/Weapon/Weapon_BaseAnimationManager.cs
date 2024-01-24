using UnityEngine;
using System.Collections;
using Project.Player;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main), typeof(Animator), typeof(AudioSource))]
    public class Weapon_BaseAnimationManager : MonoBehaviour
    {
        private Player_Movement player_Movement;
        private Player_Jump player_Jump;
        private Animator animator;
        private Weapon_Ammo weapon_Ammo;
        private Weapon_Main weaponMain;
        private Weapon_WeaponType weaponType;
        private Weapon_BaseWeaponFire baseFire;
        private Weapon_ShotgunFire shotgunFire;

        //Weapon Fire Animation
        private Transform fireAnimTransform; //Cannot use current transform since it is being used by animations.
        public AnimationCurve recoilCurvePosZ;
        public AnimationCurve recoilCurveRotY;
        [SerializeField] private float animationTime = 0.1f; // Duration of the animation
        private bool isAnimating = false;
        private Vector3 originalLocalPosition;
        private Quaternion originalLocalRotation;
        [SerializeField] private AudioClip[] foleySounds;

        //Falling related variables
        public float fallTime;
        float fallThreshold = 0.2f;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip equipSound;

        private void Awake()
        {
            fireAnimTransform = GetComponentInParent<Transform>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            weaponType = GetComponent<Weapon_WeaponType>();
            weaponMain = GetComponent<Weapon_Main>();
            weapon_Ammo = GetComponent<Weapon_Ammo>();

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

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

            if (weaponType.currentFireMode == Weapon_WeaponType.WeaponFireModes.shotgun)
            {
                shotgunFire = GetComponent<Weapon_ShotgunFire>();
                shotgunFire.OnFire += WeaponFireAnimation;
            }
            else
            {
                baseFire = GetComponent<Weapon_BaseWeaponFire>();
                baseFire.OnFire += WeaponFireAnimation;
            }

            // The original local position and rotation
            originalLocalPosition = Vector3.zero;
            originalLocalRotation = new Quaternion(0, 0, 0, 0);
            originalLocalPosition = fireAnimTransform.localPosition;
            originalLocalRotation = fireAnimTransform.localRotation;
        }

        private void OnDestroy()
        {
            if (weaponType.currentFireMode == Weapon_WeaponType.WeaponFireModes.shotgun)
            {
                shotgunFire.OnFire -= WeaponFireAnimation;
            }
            else
            {
                baseFire.OnFire -= WeaponFireAnimation;
            }
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

        //Called via Animation Event
        void PlayEquipSound()
        {
            audioSource.PlayOneShot(equipSound);
        }

        void PlayFoleySound()
        {
            audioSource.PlayOneShot(foleySounds[Random.Range(0, foleySounds.Length)]);
        }

        #region - Procedural Fire Animation -
        //!!Event based. Called via Weapon_Main Fire Method!!
        void WeaponFireAnimation()
        {
            if (weaponType.currentFireMode == Weapon_WeaponType.WeaponFireModes.automatic)
            {
                if (!isAnimating)
                {
                    StartCoroutine(FireAnimation());
                }
            }
            else if (weaponType.currentFireMode == Weapon_WeaponType.WeaponFireModes.semiautomatic)
            {
                StopCoroutine(FireAnimation()); // Stop if already running
                StartCoroutine(FireAnimation()); // Start a new instance
            }
        }

        private IEnumerator FireAnimation()
        {
            animator.SetBool("isFiring", true);
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
            animator.SetBool("isFiring", false);
        }

        public void ResetToOriginalTransform()
        {
            fireAnimTransform.localPosition = originalLocalPosition;
            fireAnimTransform.localRotation = originalLocalRotation;
        }
        #endregion

        private void Update()
        {
            switch (weaponType.currentFireMode)
            {
                case Weapon_WeaponType.WeaponFireModes.automatic:
                    animator.SetBool("WeaponIsAuto", weaponType.currentFireMode == Weapon_WeaponType.WeaponFireModes.automatic);
                    //animator.SetBool("WeaponIsFired", weaponMain.weaponIsFiring);
                    animator.SetFloat("timeSinceFire", weaponMain.timeSinceFire);
                    animator.SetBool("Reloading", weaponMain.weaponIsReloading);
                    //animator.SetInteger("AmmoCount", weaponMain.ammoCount);
                    break;
                case Weapon_WeaponType.WeaponFireModes.semiautomatic:
                    //if (weaponMain.weaponIsFiring)
                    //{
                    //    animator.SetTrigger("WeaponFireTrigger");
                    //}
                    //else if (!weaponMain.weaponIsFiring && weaponMain.timeSinceFire > 0.05f)
                    //{
                    //    animator.ResetTrigger("WeaponFireTrigger");
                    //}
                    //animator.SetBool("SemiAutoHasInput", weaponMain.newFireSemi > 0);
                    animator.SetFloat("timeSinceFire", weaponMain.timeSinceFire);
                    //animator.SetBool("WeaponIsFired", weaponMain.weaponIsFiring);
                    animator.SetBool("Reloading", weaponMain.weaponIsReloading);
                    //animator.SetInteger("AmmoCount", weaponMain.ammoCount);
                    break;
                case Weapon_WeaponType.WeaponFireModes.shotgun:
                    //if (weapon_Ammo == null)
                    //{
                    //    Debug.LogWarning("Please attach Weapon Ammo to this component!!");
                    //    return;
                    //}

                    //// Update the ammo count in the animator
                    //animator.SetInteger("AmmoCount", weapon_Ammo.ammoInMag);
                    //animator.SetBool("SemiAutoHasInput", weaponMain.newFireSemi > 0);
                    //animator.SetFloat("timeSinceFire", weaponMain.timeSinceFire);
                    //animator.SetBool("CanFire", weaponMain.allowedToFire);
                    //animator.SetBool("WeaponIsFired", weaponMain.weaponIsFiring);
                    break;
            }
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