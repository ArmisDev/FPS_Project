using UnityEngine;
using Project.Weapon;

namespace Project.Interaction
{
    public class AmmoPickup : MonoBehaviour
    {
        private InteractableObject interactableObject;
        [SerializeField]
        private enum AmmoType
        {
            assualt,
            pistol,
            shotgun
        }

        private AmmoType currentAmmoType
        {
            get => CurrentAmmoType;
            set
            {
                CurrentAmmoType = value;
            }
        }

        [SerializeField]
        private AmmoType CurrentAmmoType;
        [SerializeField] private int ammoAmount;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickupSound;

        private void Awake()
        {
            interactableObject = GetComponent<InteractableObject>();
            interactableObject.OnInteractWithObject += HandlePickup;
            audioSource = GetComponent<AudioSource>();
        }

        private void OnDestroy()
        {
            interactableObject.OnInteractWithObject -= HandlePickup;
        }

        void HandlePickup()
        {
            interactableObject.interactionSender.TryGetComponent(out Weapon_Inventory newInventory);
            if (newInventory == null)
            {
                Debug.Log("Could not find Weapon_Inventory");
                return;
            }
            newInventory.currentWeapon.TryGetComponent(out Weapon_Ammo ammo);
            newInventory.currentWeapon.TryGetComponent(out Weapon_WeaponType weaponType);

            if (ammo == null)
            {
                Debug.Log("Could not find Weapon_Ammo from " + newInventory.currentWeapon.name);
                return;
            }
            if(weaponType == null)
            {
                Debug.Log("Could not find Weapon_WeaponType from " + newInventory.currentWeapon.name);
                return;
            }

            //Grab string references for ammo types
            string ammoPickupType = currentAmmoType.ToString();
            string weaponAmmoType = weaponType.currentweaponCaliber.ToString();

            if (ammoPickupType == weaponAmmoType)
            {
                ammo.ammoInventory += ammoAmount;
                audioSource.PlayOneShot(pickupSound);
            }
        }
    }
}