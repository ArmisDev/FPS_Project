using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Player;

namespace Project.Weapon
{
    [RequireComponent(typeof(PlayerInput))]
    public class Weapon_Main : MonoBehaviour
    {
        private Player_Movement player_Movement;
        [HideInInspector] public GameObject firePoint;
        private PlayerInput input;

        [Header("Weapon States")]
        //[SerializeField] private bool isOnlySemiAutomatic;
        [SerializeField] private bool canSwitchFireModes;
        [HideInInspector] public bool weaponIsFiring;
        [HideInInspector] public bool weaponIsReloading;
        [HideInInspector] public bool allowedToFire; //Set by operator in code (see ShotgunReset coroutine for example).
                                                    //Differs from canFire() because this is set by operator.
        private bool isAiming;

        //States
        public enum WeaponFireModes
        {
            automatic,
            semiautomatic,
            shotgun
        }

        public WeaponFireModes currentFireMode;
        private List<WeaponFireModes> fireModes = new List<WeaponFireModes>();

        [Header("Weapon Recoil")]
        public float recoilAmount; //Public so recoil script can access

        [Header("Weapon Fire")]
        [HideInInspector] public float range;
        [SerializeField] private float damage;
        [SerializeField] private float fireRate;
        //public RaycastHit raycastHit;
        [HideInInspector] public float timeSinceFire;
        public LayerMask rayExclusionLayer;

        [Header("Weapon Ammo")]
        [SerializeField] private float reloadTime;
        public int ammoCount;
        public int maxAmmo;
        private Coroutine reloadCoroutine;

        [Header("Aim In")]
        [SerializeField] private Vector3 desiredPos;
        [SerializeField] private Vector3 desiredRotation;
        [SerializeField] private float aimInAccel;
        private Transform aimInTransform;
        private Vector3 initPos;
        private Vector3 initRotation;
        public float aimInIncrement;

        [Header("Input Logic")]
        [HideInInspector] public float recoveryTimeThreshold; // The maximum amount of time allowed after the last shot before setting "weaponIsFiring" to false.
        private float recoveryTimeTolerance; // Tolerence on when isFiring should be set to false
        private InputAction fireSemi;
        private InputAction fireAuto;
        private InputAction reload;
        private InputAction fireModeSwitch;
        private InputAction aimIn;

        //Private New Float Values
        [HideInInspector] public float newFireAuto;
        [HideInInspector] public float newFireSemi;
        [HideInInspector] public float newFireModeSwitch;

        //Event
        public event Action OnFire;
        public event Action OnReload;

        private void Awake()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            firePoint = GameObject.FindGameObjectWithTag("MainCamera");

            if (playerObject != null)
            {
                player_Movement = playerObject.GetComponent<Player_Movement>();
            }
            else
            {
                Debug.Log("player_Movement cannot be found, make sure the player is tagged as Player!!");
            }

            //Input Assignment
            input = GetComponent<PlayerInput>();
            fireSemi = input.actions["Fire_Semi"];
            fireAuto = input.actions["Fire_Auto"];
            reload = input.actions["Reload"];
            fireModeSwitch = input.actions["FireModeSwitch"];
            aimIn = input.actions["AimIn"];

            //Add Firemodes to list
            fireModes.Add(WeaponFireModes.automatic);
            fireModes.Add(WeaponFireModes.semiautomatic);

            recoveryTimeTolerance = recoveryTimeThreshold; //Ensures that the weapon can fire at runtime.

            //Find and set components
            GameObject aimInObj = GameObject.FindGameObjectWithTag("ObjectHolder");
            aimInTransform = aimInObj.transform;
            initPos = aimInTransform.localPosition;
            initRotation = aimInTransform.localEulerAngles;

            //Make sure we are initially allowed to fire
            allowedToFire = true;
        }

        #region - Fire Logic -
        //Decided by current weapon aspects. Not set by operator
        public bool CanFire()
        {
            return ammoCount > 0;
        }

        public void WeaponFire()
        {
            Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out RaycastHit hit, range, ~rayExclusionLayer);
            Debug.DrawRay(firePoint.transform.position, firePoint.transform.forward * range, Color.green, 1f);
            weaponIsFiring = true;
            timeSinceFire = 0;

            //FX & Events=
            OnFire?.Invoke();
            ammoCount--; //We want to decrease ammo after fire event has been invoked.
                         //This ensures that if we want to do something when our weapon has one round in it, that one round doesn't get removed before the event call.

            if (hit.collider == null) return;
            HitRigidBodyCheck(hit);
        }

        public void ShotgunWeaponFire()
        {
            weaponIsFiring = true;
            timeSinceFire = 0; //This should be called before Event!

            OnFire?.Invoke(); //Additional fire logic is handled by Weapon_ShotgunFire on OnFire event.

            ammoCount--; //We want to decrease ammo after fire event has been invoked.
                         //This ensures that if we want to do something when our weapon has one round in it, that one round doesn't get removed before the event call.
        }

        IEnumerator ShotGunReset()
        {
            allowedToFire = false;
            yield return new WaitForSeconds(recoveryTimeThreshold);
            allowedToFire = true;
            weaponIsFiring = false;
        }

        //Rigidbody Check for semi and auto weapons
        public void HitRigidBodyCheck(RaycastHit hit)
        {
            hit.collider.TryGetComponent(out Rigidbody objectRigidbody);
            if (objectRigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * damage, ForceMode.Impulse);
            }
        }
        #endregion

        #region - Reload Logic -
        void ReloadWeapon()
        {
            switch(currentFireMode)
            {
                case WeaponFireModes.automatic:
                    if (reload.ReadValue<float>() > 0 && !weaponIsFiring && !weaponIsReloading)
                    {
                        if (reloadCoroutine != null)
                            StopCoroutine(reloadCoroutine);

                        reloadCoroutine = StartCoroutine(ReloadCoroutine());
                    }
                    break;
                case WeaponFireModes.semiautomatic:
                    if (reload.ReadValue<float>() > 0 && !weaponIsFiring && !weaponIsReloading)
                    {
                        if (reloadCoroutine != null)
                            StopCoroutine(reloadCoroutine);

                        reloadCoroutine = StartCoroutine(ReloadCoroutine());
                    }
                    break;
                case WeaponFireModes.shotgun:

                    if (reload.ReadValue<float>() > 0 && !weaponIsFiring && !weaponIsReloading)
                    {
                        if (ammoCount == maxAmmo) return;
                        weaponIsReloading = true;
                    }
                    
                    else if(weaponIsReloading && ammoCount == maxAmmo)
                    {
                        weaponIsReloading = false;
                        allowedToFire = true;
                    }

                    while (weaponIsReloading)
                    {
                        allowedToFire = false;
                        break;
                    }
                    break;
            }
        }

        public void StopReload()
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = null;
                weaponIsReloading = false;
            }
        }

        //Adjusts weapon states, waits for a specified amount of time, and then re adjusts states.
        IEnumerator ReloadCoroutine()
        {
            weaponIsReloading = true;
            OnReload?.Invoke();
            yield return new WaitForSeconds(reloadTime);
            ammoCount = maxAmmo;
            weaponIsReloading = false;
            reloadCoroutine = null;
        }
        #endregion

        #region - Aim In Logic -
        void AimInandOutLogic()
        {
            float aimInputValue = aimIn.ReadValue<float>();
            bool isAimingInput = aimInputValue > 0;

            if (isAimingInput && !player_Movement.isRunning)
            {
                if (!isAiming)
                {
                    // Reset increment when starting to aim in
                    aimInIncrement = 0;
                }

                aimInIncrement += Time.deltaTime * aimInAccel;
                aimInIncrement = Mathf.Clamp(aimInIncrement, 0, 1); // Clamp to ensure it stays within range

                UpdateAimTransform(true);
                isAiming = true;
            }
            else if (!isAimingInput && isAiming)
            {
                // Aim out logic
                aimInIncrement -= Time.deltaTime * aimInAccel;
                aimInIncrement = Mathf.Clamp(aimInIncrement, 0, 1); // Clamp to ensure it stays within range

                UpdateAimTransform(false);

                // Check if the initial position is reached
                if (aimInIncrement == 0)
                {
                    isAiming = false;
                }
            }
            else if (aimInputValue == 0 && !isAiming)
            {
                // No aiming input and not currently aiming
                aimInIncrement = 0;
            }
        }

        void UpdateAimTransform(bool isAimingIn)
        {
            Vector3 targetPosition = isAimingIn ? desiredPos : initPos;
            Vector3 targetRotation = isAimingIn ? desiredRotation : initRotation;

            aimInTransform.localPosition = Vector3.Lerp(
                aimInTransform.localPosition, targetPosition, aimInIncrement);

            aimInTransform.localEulerAngles = Vector3.Lerp(
                aimInTransform.localEulerAngles, targetRotation, aimInIncrement);
        }
        #endregion

        private void Update()
        {
            AimInandOutLogic();
            CanFire(); //Always check if we can fire first
            timeSinceFire += Time.deltaTime; //Constantly update are timesincefire.
            ReloadWeapon();

            //Read Inputs
            newFireAuto = fireAuto.ReadValue<float>();
            newFireSemi = fireSemi.ReadValue<float>();
            newFireModeSwitch = fireModeSwitch.ReadValue<float>();

            #region - Fire Logic Input/State Detection -
            //!!!Fire Logic!!!
            //Switch statements handles different firing logic depending on the current fire mode.
            //!!!Must be executed before fire mode change!!!
            switch (currentFireMode)
            {
                case WeaponFireModes.automatic:
                    if (newFireAuto > 0 && timeSinceFire >= fireRate && CanFire() && !weaponIsReloading)
                    {
                        WeaponFire();
                    }
                    else if (newFireAuto == 0 || !CanFire())
                    {
                        weaponIsFiring = false; //If no input is happening, then we are not firing weapon.
                                                //weapon_Recoil.ResetWeaponRotation();
                                                //When we are not firing we want to make sure that are weapon resets back to its original pos.
                    }
                    break;
                case WeaponFireModes.semiautomatic:
                    if (fireSemi.WasPerformedThisFrame() && CanFire() && !weaponIsReloading)
                    {
                        WeaponFire();
                        recoveryTimeTolerance = recoveryTimeThreshold; // Reset the timer
                    }
                    else
                    {
                        if (recoveryTimeTolerance > 0)
                        {
                            recoveryTimeTolerance -= Time.deltaTime; // Decrement the timer
                        }
                        else
                        {
                            weaponIsFiring = false; // Set to false only when timer reaches zero
                        }
                    }
                    break;
                case WeaponFireModes.shotgun:
                    if(fireSemi.WasPerformedThisFrame() && CanFire() && allowedToFire && !weaponIsReloading)
                    {
                        ShotgunWeaponFire();
                        StartCoroutine(ShotGunReset());
                    }
                    break;
            }
            #endregion

            #region - FireMode Switch -
            if (canSwitchFireModes)
            {
                if (!fireModeSwitch.WasPressedThisFrame()) return;
                else
                {
                    if (canSwitchFireModes && newFireModeSwitch > 0)
                    {
                        // Find the index of the current fire mode
                        int currentModeIndex = fireModes.IndexOf(currentFireMode);

                        // Increment the index to switch to the next fire mode
                        int nextModeIndex = (currentModeIndex + 1) % fireModes.Count;

                        // Set the current fire mode to the next one in the list
                        currentFireMode = fireModes[nextModeIndex];

                        Debug.Log("Weapon state eqauls " + currentFireMode);
                    }
                }
            }
            #endregion
        }
    }
}