using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main), typeof(Weapon_WeaponType), typeof(Weapon_HitCheck))]
    public class Weapon_BaseWeaponFire : MonoBehaviour
    {
        private Weapon_WeaponType weaponType;
        private Weapon_Main weaponMain;
        private WeaponAudio weaponAudio;
        private Weapon_Ammo weaponAmmo;
        private Weapon_HitCheck hitCheck;
        private Weapon_Inventory weaponInventory;
        public event Action OnFire;
        public event Action OnReload;
        public Coroutine reloadCoroutine;

        private List<Weapon_WeaponType.WeaponFireModes> fireModes = new List<Weapon_WeaponType.WeaponFireModes>();

        private void Awake()
        {
            weaponInventory = GetComponentInParent<Weapon_Inventory>();
            weaponAmmo = GetComponent<Weapon_Ammo>();

            weaponAudio = GetComponent<WeaponAudio>();
            if (weaponAudio == null) Debug.LogError("Please attach WeaponAudio to component to " + name);

            weaponMain = GetComponent<Weapon_Main>();
            weaponType = GetComponent<Weapon_WeaponType>();
            hitCheck = GetComponent<Weapon_HitCheck>();

            //Add Firemodes to list
            fireModes.Add(Weapon_WeaponType.WeaponFireModes.automatic);
            fireModes.Add(Weapon_WeaponType.WeaponFireModes.semiautomatic);
        }

        public bool CanFire()
        {
            return weaponMain.ammoCount > 0;
        }

        public void WeaponFire()
        {
            Physics.Raycast(weaponMain.firePoint.transform.position, weaponMain.firePoint.transform.forward, out RaycastHit hit, weaponMain.range, ~weaponMain.rayExclusionLayer);
            Debug.DrawRay(weaponMain.firePoint.transform.position, weaponMain.firePoint.transform.forward * weaponMain.range, Color.green, 1f);
            weaponMain.weaponIsFiring = true;
            weaponMain.timeSinceFire = 0;

            //FX & Events
            //We want to decrease ammo after fire event has been invoked.
            //This ensures that if we want to fire when our weapon has one round in it, that one round doesn't get removed before the event call.
            OnFire?.Invoke();
            weaponAmmo.ammoInMag--;
            weaponAmmo.roundsFired++;

            //Audio
            //Here we check to see if we should play the last shot audioclip or not
            if(weaponAmmo.ammoInMag > 0) weaponAudio.PlayWeaponFireAudio(false);
            else weaponAudio.PlayWeaponFireAudio(true);

            if (hit.collider == null) return;
            hitCheck.RayHitCheck(hit);
        }

        void ReloadWeapon()
        {
            //Check to see if we have hit reload button, we're not firing, not reloading, and our inventory has ammo before we attempt to reload
            if (weaponMain.reload.ReadValue<float>() > 0 && !weaponMain.weaponIsFiring && !weaponMain.weaponIsReloading && weaponAmmo.ammoInventory > 0)
            {
                if (reloadCoroutine != null)
                    StopCoroutine(reloadCoroutine);

                reloadCoroutine = StartCoroutine(ReloadCoroutine());
                weaponAudio.PlayReloadAudio();
            }
        }

        public void StopReload()
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = null;
                weaponMain.weaponIsReloading = false;
            }
        }

        private void AdjustAmmoInInventory()
        {
            weaponAmmo.ammoInventory -= weaponAmmo.roundsFired;
            Debug.Log(weaponAmmo.ammoInventory);
            weaponAmmo.roundsFired = 0;
        }

        //Adjusts weapon states, waits for a specified amount of time, and then re adjusts states.
        IEnumerator ReloadCoroutine()
        {
            weaponMain.weaponIsReloading = true;
            OnReload?.Invoke();
            yield return new WaitForSeconds(weaponMain.reloadTime);

            // Calculate the amount of ammo needed to fully reload the magazine
            int ammoNeededForReload = weaponMain.maxAmmo - weaponAmmo.ammoInMag;

            // If there's more ammo in the inventory than needed, reload to max capacity and adjust inventory
            if (weaponAmmo.ammoInventory >= ammoNeededForReload)
            {
                weaponAmmo.ammoInMag += ammoNeededForReload;
                weaponAmmo.ammoInventory -= ammoNeededForReload;
            }
            // If there's less ammo in the inventory than needed, reload with whatever is available
            else
            {
                weaponAmmo.ammoInMag += weaponAmmo.ammoInventory;
                weaponAmmo.ammoInventory = 0;
            }

            weaponMain.weaponIsReloading = false;
            reloadCoroutine = null;
        }


        // Update is called once per frame
        void Update()
        {
            if (weaponMain.stopWeaponLogic) return;

            //Read Inputs
            weaponMain.newFireAuto = weaponMain.fireAuto.ReadValue<float>();
            weaponMain.newFireSemi = weaponMain.fireSemi.ReadValue<float>();
            weaponMain.newFireModeSwitch = weaponMain.fireModeSwitch.ReadValue<float>();

            CanFire(); //Always check if we can fire first
            weaponMain.timeSinceFire += Time.deltaTime; //Constantly update are timesincefire.
            ReloadWeapon();

            #region - FireMode Switch -
            //if (weaponMain.canSwitchFireModes)
            //{
            //    if (!weaponMain.fireModeSwitch.WasPressedThisFrame()) return;
            //    else
            //    {
            //        if (weaponMain.canSwitchFireModes && weaponMain.newFireModeSwitch > 0)
            //        {
            //            // Find the index of the current fire mode
            //            int currentModeIndex = fireModes.IndexOf(weaponType.currentFireMode);

            //            // Increment the index to switch to the next fire mode
            //            int nextModeIndex = (currentModeIndex + 1) % fireModes.Count;

            //            // Set the current fire mode to the next one in the list
            //            weaponType.currentFireMode = fireModes[nextModeIndex];

            //            Debug.Log("Weapon state eqauls " + weaponType.currentFireMode);
            //        }
            //    }
            //}
            if (weaponMain.canSwitchFireModes && weaponMain.fireModeSwitch.WasPressedThisFrame())
            {
                if (weaponMain.canSwitchFireModes && weaponMain.newFireModeSwitch > 0)
                {
                    // Find the index of the current fire mode
                    int currentModeIndex = fireModes.IndexOf(weaponType.currentFireMode);

                    // Increment the index to switch to the next fire mode
                    int nextModeIndex = (currentModeIndex + 1) % fireModes.Count;

                    // Set the current fire mode to the next one in the list
                    weaponType.currentFireMode = fireModes[nextModeIndex];

                    Debug.Log("Weapon state eqauls " + weaponType.currentFireMode);
                }
            }
            #endregion

            //First check if we are trying to dry fire
            if (weaponAmmo.ammoInMag <= 0)
            {
                if(weaponMain.fireSemi.WasPerformedThisFrame())
                {
                    weaponAudio.PlayOutOfAmmo();
                }
                weaponMain.weaponIsFiring = false;
                weaponMain.allowedToFire = false;
                weaponAmmo.roundsFired = 0;
                return;
            }

            switch (weaponType.currentFireMode)
            {
                case Weapon_WeaponType.WeaponFireModes.automatic:
                    if (weaponMain.newFireAuto > 0 && weaponMain.timeSinceFire >= weaponMain.fireRate && CanFire() && !weaponMain.weaponIsReloading)
                    {
                        weaponMain.allowedToFire = true;
                        WeaponFire();
                    }
                    else if (weaponMain.newFireAuto == 0 || !CanFire())
                    {
                        weaponMain.weaponIsFiring = false; //If no input is happening, then we are not firing weapon.
                                                //weapon_Recoil.ResetWeaponRotation();
                                                //When we are not firing we want to make sure that are weapon resets back to its original pos.
                    }
                    break;
                case Weapon_WeaponType.WeaponFireModes.semiautomatic:
                    if (weaponMain.fireSemi.WasPerformedThisFrame() && !weaponMain.weaponIsReloading)
                    {
                        weaponMain.allowedToFire = true;
                        WeaponFire();
                        weaponMain.recoveryTimeTolerance = weaponMain.recoveryTimeThreshold; // Reset the timer
                    }
                    else
                    {
                        if (weaponMain.recoveryTimeTolerance > 0)
                        {
                            weaponMain.recoveryTimeTolerance -= Time.deltaTime; // Decrement the timer
                        }
                        else
                        {
                            weaponMain.weaponIsFiring = false; // Set to false only when timer reaches zero
                        }
                    }
                    break;
            }
        }
    }
}