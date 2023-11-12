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
        private GameObject firePoint;
        private PlayerInput input;

        [Header("Weapon States")]
        [SerializeField] private bool isOnlySemiAutomatic;
        [SerializeField] private bool canSwitchFireModes;
        [HideInInspector] public bool weaponIsFiring;
        [HideInInspector] public bool weaponIsReloading;

        [Header("Weapon Recoil")]
        public float recoilAmount; //Public so recoil script can access

        [Header("Weapon Fire")]
        [SerializeField] private float range;
        [SerializeField] private float damage;
        [SerializeField] private float fireRate;

        //Private / Hidden Weapon Fire Variables
        private RaycastHit raycastHit;
        [HideInInspector] public float timeSinceFire;

        [Header("Weapon Ammo")]
        [SerializeField] private float reloadTime;
        public float ammoCount;
        [SerializeField] private float maxAmmo;
        private Coroutine reloadCoroutine;

        [Header("Input Logic")]
        [SerializeField] private float semiAutoFireThreshold; // The maximum amount of time allowed after the last shot before setting "weaponIsFiring" to false.
        private float semiAutoFireTolerance; // Tolerence on when isFiring should be set to false
        private InputAction fireSemi;
        private InputAction fireAuto;
        private InputAction reload;
        private InputAction fireModeSwitch;

        //Private New Float Values
        [HideInInspector] public float newFireAuto;
        [HideInInspector] public float newFireSemi;
        [HideInInspector] public float newFireModeSwitch;

        //States
        public enum WeaponFireModes
        {
            automatic,
            semiautomatic
        }

        [HideInInspector] public WeaponFireModes currentFireMode;
        private List<WeaponFireModes> fireModes = new List<WeaponFireModes>();

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

            //Weapon state check
            if (isOnlySemiAutomatic)
            {
                canSwitchFireModes = false;
                currentFireMode = WeaponFireModes.semiautomatic;
            }
            else currentFireMode = WeaponFireModes.automatic;

            //Input Assignment
            input = GetComponent<PlayerInput>();
            fireSemi = input.actions["Fire_Semi"];
            fireAuto = input.actions["Fire_Auto"];
            reload = input.actions["Reload"];
            fireModeSwitch = input.actions["FireModeSwitch"];

            //Add Firemodes to list
            fireModes.Add(WeaponFireModes.automatic);
            fireModes.Add(WeaponFireModes.semiautomatic);

            semiAutoFireTolerance = semiAutoFireThreshold; //Ensures that the weapon can fire at runtime.
        }

        public bool CanFire()
        {
            return ammoCount > 0;
        }

        public void WeaponFire()
        {
            Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out raycastHit, range);
            weaponIsFiring = true;
            timeSinceFire = 0;
            ammoCount--;

            //FX & Events
            OnFire?.Invoke();
        }

        void ReloadWeapon()
        {
            if (reload.ReadValue<float>() > 0 && !weaponIsFiring && !weaponIsReloading)
            {
                if (reloadCoroutine != null)
                    StopCoroutine(reloadCoroutine);

                reloadCoroutine = StartCoroutine(ReloadCoroutine());
            }
        }

        public void StopReload()
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = null;
                weaponIsReloading = false;
                // Additional cleanup if necessary
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

        private void Update()
        {
            CanFire(); //Always check if we can fire first
            timeSinceFire += Time.deltaTime; //Constantly update are timesincefire.
            ReloadWeapon();

            //Read Inputs
            newFireAuto = fireAuto.ReadValue<float>();
            newFireSemi = fireSemi.ReadValue<float>();
            newFireModeSwitch = fireModeSwitch.ReadValue<float>();

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
                    if (fireSemi.WasPerformedThisFrame() && newFireSemi > 0 && CanFire() && !weaponIsReloading)
                    {
                        WeaponFire();
                        semiAutoFireTolerance = semiAutoFireThreshold; // Reset the timer
                    }
                    else
                    {
                        if (semiAutoFireTolerance > 0)
                        {
                            semiAutoFireTolerance -= Time.deltaTime; // Decrement the timer
                        }
                        else
                        {
                            weaponIsFiring = false; // Set to false only when timer reaches zero
                        }
                    }
                    break;
            }

            //Firemode change
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
        }
    }
}