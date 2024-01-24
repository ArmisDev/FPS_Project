using System.Collections;
using System;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main), typeof(AudioSource), typeof(Weapon_HitCheck))]
    public class Weapon_ShotgunFire : MonoBehaviour
    {
        private Weapon_Main weaponMain;
        private Weapon_Ammo weaponAmmo;
        private WeaponAudio weaponAudio;
        private Weapon_HitCheck hitCheck;
        private Weapon_Inventory weaponInventory;
        private Animator animator;
        public bool isLastShell;
        [SerializeField] private int pellets;
        [SerializeField] private float spread;

        private AudioSource audioSource;
        [SerializeField] private AudioClip pumpRoundClip;
        [SerializeField] private AudioClip loadShellClip;

        public event Action OnFire;
        public event Action OnReload;

        private void Awake()
        {
            weaponInventory = GetComponentInParent<Weapon_Inventory>();
            weaponAmmo = GetComponent<Weapon_Ammo>();
            weaponMain = GetComponent<Weapon_Main>();
            hitCheck = GetComponent<Weapon_HitCheck>();
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            weaponMain.input = GetComponent<PlayerInput>();

            weaponAudio = GetComponent<WeaponAudio>();
            if (weaponAudio == null) Debug.LogError("Please Attach Weapon Audio script to component");

            weaponMain.fireSemi = weaponMain.input.actions["Fire_Semi"];
            weaponMain.fireAuto = weaponMain.input.actions["Fire_Auto"];
            weaponMain.reload = weaponMain.input.actions["Reload"];
            weaponMain.fireModeSwitch = weaponMain.input.actions["FireModeSwitch"];

            //Set our ammo count to how much we have in our inventory
            //If we have more ammo in our inventory than our max will allow, default the reload count to the max ammount
            //Else, just use what we have in our inventory.
            if (weaponInventory.inventoryAmmo > weaponAmmo.maxAmmo)
            {
                weaponAmmo.ammoInMag = weaponAmmo.maxAmmo;
            }
            else weaponAmmo.ammoInMag = weaponInventory.inventoryAmmo;
        }

        public bool CanFire()
        {
            return weaponAmmo.ammoInMag > 0;
        }

        void Raycastlogic()
        {
            for (int i = 0; i < Mathf.Max(1, pellets); i++)
            {
                // Calculate the forward direction
                Vector3 forward = weaponMain.firePoint.transform.forward;

                // Add randomness to the direction
                float randomX = UnityEngine.Random.Range(-spread, spread);
                float randomY = UnityEngine.Random.Range(-spread, spread);
                Vector3 randomDirection = forward + new Vector3(randomX, randomY, 0);

                // Normalize the direction to maintain consistent length
                randomDirection.Normalize();

                // Calculate the final target point
                Vector3 f_branch = weaponMain.firePoint.transform.position + randomDirection * 1000f;
                Physics.Raycast(weaponMain.firePoint.transform.position, f_branch - weaponMain.firePoint.transform.position, out RaycastHit hit, weaponMain.range, ~weaponMain.rayExclusionLayer);
                Debug.DrawRay(weaponMain.firePoint.transform.position, f_branch - weaponMain.firePoint.transform.position, Color.green, 1f);

                if (hit.collider == null) return;
                hitCheck.RayHitCheck(hit);
            }
        }

        public void ShotgunWeaponFire()
        {
            weaponMain.weaponIsFiring = true;
            weaponMain.timeSinceFire = 0; //This should be called before Event!
            Raycastlogic();

            //Check ammo to play proper audio
            if (weaponAmmo.ammoInMag > 1) weaponAudio.PlayWeaponFireAudio(false);
            else weaponAudio.PlayWeaponFireAudio(true);

            OnFire?.Invoke();
            weaponAmmo.ammoInMag--;
            weaponAmmo.roundsFired++;
            //We want to decrease ammo after fire event has been invoked.
            //This ensures that if we want to do something when our weapon has one round in it, that one round doesn't get removed before the event call.
        }

        IEnumerator ShotGunReset()
        {
            weaponMain.allowedToFire = false;
            yield return new WaitForSeconds(weaponMain.recoveryTimeThreshold);
            weaponMain.allowedToFire = true;
            weaponMain.weaponIsFiring = false;
        }

        //Animation Events
        //Called via Animation Event
        void PlayPumpRoundClip()
        {
            PlayAudio(pumpRoundClip);
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

        void BeginLoad()
        {
            animator.SetBool("StartReload 0", false);
            animator.SetBool("Reloading", true);
        }

        void AddShellForShotgun()
        {
            if (weaponAmmo.ammoInMag < weaponAmmo.maxAmmo && weaponAmmo.ammoInventory > 0 && weaponMain.weaponIsReloading)
            {
                weaponAmmo.ammoInMag++;
                weaponAmmo.ammoInventory--;

                if (!isLastShell)
                {
                    animator.SetBool("Reloading", true);
                }
                else
                {
                    animator.SetBool("Reloading", false);
                    animator.SetBool("ReloadLastShell", true);
                }
            }
            else
            {
                // Stop reloading if magazine is full or no more ammo in the inventory
                weaponMain.weaponIsReloading = false;
            }
        }

        void FinishedReload()
        {
            // Stop reloading if magazine is full or no more ammo in the inventory
            animator.SetBool("StartReload 0", false);
            animator.SetBool("Reloading", false);
            animator.SetBool("ReloadLastShell", false);
            weaponMain.weaponIsReloading = false;
        }

        void ReloadShotgun()
        {
            // Check if it's the last shell to be loaded or if the inventory is empty
            if (weaponMain.reload.WasPerformedThisFrame() && !weaponMain.weaponIsFiring && !weaponMain.weaponIsReloading && weaponAmmo.ammoInventory > 0 && weaponAmmo.ammoInMag != weaponAmmo.maxAmmo)
            {
                Debug.Log("Reload Test");
                weaponMain.weaponIsReloading = true;
                animator.SetBool("Reloading", false);
                animator.SetBool("StartReload 0", true);
            }
        }

        void Update()
        {
            if (weaponMain.stopWeaponLogic) return;
            //Last shell logic
            isLastShell = weaponAmmo.ammoInMag == weaponAmmo.maxAmmo - 1 || weaponAmmo.ammoInventory == 1;

            CanFire(); //Always check if we can fire first
            weaponMain.timeSinceFire += Time.deltaTime; //Constantly update are timesincefire.
            //Check for reload
            if (weaponMain.weaponIsReloading)
            {
                animator.SetBool("ReloadLastShell", isLastShell);
            }
            ReloadShotgun();
            if (weaponMain.fireSemi.WasPerformedThisFrame() && CanFire() && weaponMain.allowedToFire && weaponAmmo.ammoInMag > 0)
            {
                ShotgunWeaponFire();
                StartCoroutine(ShotGunReset());
                weaponMain.weaponIsReloading = false;
            }
        }
    }
}