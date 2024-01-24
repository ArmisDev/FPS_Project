using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Interaction;

namespace Project.Weapon
{
    [RequireComponent(typeof(PlayerInput), typeof(Interaction_Main))]
    public class Weapon_Inventory : MonoBehaviour
    {
        [Header("Inventory Parameters")]
        public Transform GunsTransform;
        public Transform DropPosition;
        public GameObject droppedWeaponsContainer;
        [SerializeField] private int inventorySize;
        [SerializeField] private int maxPrimary;
        [SerializeField] private int maxSecondary;
        [SerializeField] private int maxSpecial;
        [HideInInspector] public bool stopWeaponSwitch;

        [Header("Ammo")]
        //private Dictionary<string, AmmoData> savedAmmoData = new();
        [HideInInspector] public int inventoryAmmo;

        //Dropped Weapon Logic
        public List<GameObject> droppedWeapons = new();

        [HideInInspector] public GameObject currentWeapon;
        private GameObject[] weapon_Mains;  // Array of weapons in the game
        public List<GameObject> weapons = new();

        private Interaction_Main interaction_Main;
        private float switchCooldown = .3f;
        private float lastSwitchTime = -1f;
        [HideInInspector] public int currentInventorySize;

        private PlayerInput input;
        private InputAction weaponSwitch;
        private InputAction dropWeapon;

        private event Action OnWeaponSwitch;

        public int currentPrimaryCount = 0;
        public int currentSecondaryCount = 0;
        public int currentSpecialCount = 0;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            weaponSwitch = input.actions["SwitchWeapons"];
            dropWeapon = input.actions["DropWeapon"];

            weapon_Mains = GameObject.FindGameObjectsWithTag("Weapon");
            interaction_Main = GetComponent<Interaction_Main>();
            interaction_Main.OnInteraction += UpdateInventory;
        }

        private void Start()
        {
            foreach (GameObject weapon_Main in weapon_Mains)
            {
                weapon_Main.SetActive(false);
                weapons.Add(weapon_Main);
            }

            if (weapons.Count > 0)
            {
                currentWeapon = weapons[0];
                currentWeapon.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            interaction_Main.OnInteraction -= UpdateInventory;
        }

        private void UpdateInventory(object sender, WeaponInteractionEventArgs e)
        {
            // Check if inventory is full
            if (weapons.Count >= inventorySize)
            {
                Debug.Log("Inventory is full");
                return;
            }

            // Check for weapon data like ammo
            GameObject weaponToAdd = Instantiate(e.Prefab, GunsTransform);
            Weapon_Main weaponToAddMain = weaponToAdd.GetComponent<Weapon_Main>();
            Weapon_Pickupable pickupableComponent = e.DroppableWeapon.GetComponent<Weapon_Pickupable>();
            Weapon_Ammo ammoComponentToAdd = weaponToAdd.GetComponent<Weapon_Ammo>();

            if (pickupableComponent != null && ammoComponentToAdd != null)
            {
                AmmoData savedAmmoData = pickupableComponent.GetAmmoData();
                ammoComponentToAdd.SetAmmoData(savedAmmoData);
            }

            weaponToAdd.SetActive(false);

            #region - Limit Check -
            // Increment the counter based on the weapon's type
            IncrementWeaponCounter(e.WeaponType);

            // Check if adding the new weapon would exceed the type limits
            bool exceedsLimit = false;
            switch (e.WeaponType)
            {
                case WeaponType.primary:
                    if (currentPrimaryCount > maxPrimary)
                    {
                        exceedsLimit = true;
                    }
                    break;
                case WeaponType.secondary:
                    if (currentSecondaryCount > maxSecondary)
                    {
                        exceedsLimit = true;
                    }
                    break;
                case WeaponType.special:
                    if (currentSpecialCount > maxSpecial)
                    {
                        exceedsLimit = true;
                    }
                    break;
            }

            if (exceedsLimit)
            {
                Weapon_WeaponType weaponType = currentWeapon.GetComponent<Weapon_WeaponType>();
                // Decrement the counter since we're not adding the weapon
                DecrementWeaponCounter(weaponType.currentWeaponType);

                interaction_Main.DestroyObjectStateSet(false); // Ensure we don't delete the pickupable weapon from the world
                Destroy(weaponToAdd); // But we do delete it from our character/inventory
                return;
            }
            #endregion

            #region - Check for Previous Weapon -
            //Grab the unique ID from the event args
            string weaponToAddID = e.UniqueID;
            //Weapon_Main weaponToAddMain = weaponToAdd.GetComponent<Weapon_Main>();

            if (weaponToAddMain != null)
            {
                weaponToAddMain.weaponID = weaponToAddID;

                // Find the dropped weapon with the same ID and handle it
                GameObject objectToReparent = null;
                foreach (GameObject droppedWeapon in droppedWeapons)
                {
                    Weapon_Main tempWeaponMain = droppedWeapon.GetComponent<Weapon_Main>();
                    if (tempWeaponMain != null && tempWeaponMain.weaponID == weaponToAddID)
                    {
                        objectToReparent = droppedWeapon;
                        break; // Found the matching weapon, no need to continue the loop
                    }
                }

                if (objectToReparent != null)
                {
                    droppedWeapons.Remove(objectToReparent); // Remove from droppedWeapons list
                    Destroy(objectToReparent); // Destroy the old dropped weapon
                                               // Optionally, you can handle any state transfer here if needed
                }
            }
            #endregion

            //Assign the droppable weapon in the main class to that of the current pickup
            //This ensures that it's unique ID remains the same
            //As well as allowing us to instantiate a weapon in DropWeapon() that has data filled out
            //Doping this ensures that we do not constantly generate different UniqueIDs when we go to pickup and drop weapons
            //... I think :)
            Weapon_Main currentWeaponMain = weaponToAdd.GetComponent<Weapon_Main>();
            currentWeaponMain.dropableObject = e.DroppableWeapon;

            // Add the new weapon to the inventory
            weapons.Add(weaponToAdd);
            currentWeapon.SetActive(false);
            currentWeapon = weaponToAdd;
            weaponToAdd.SetActive(true);
            interaction_Main.DestroyObjectStateSet(true);

            // Restore the state of the weapon
            Weapon_Pickupable pickupable = weaponToAdd.GetComponent<Weapon_Pickupable>();
        }

        private void IncrementWeaponCounter(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.primary:
                    currentPrimaryCount++;
                    break;
                case WeaponType.secondary:
                    currentSecondaryCount++;
                    break;
                case WeaponType.special:
                    currentSpecialCount++;
                    break;
            }
        }

        private void DecrementWeaponCounter(Weapon_WeaponType.WeaponType type)
        {
            switch (type)
            {
                case Weapon_WeaponType.WeaponType.primary:
                    currentPrimaryCount--;
                    break;
                case Weapon_WeaponType.WeaponType.secondary:
                    currentSecondaryCount--;
                    break;
                case Weapon_WeaponType.WeaponType.special:
                    currentSpecialCount--;
                    break;
            }
        }

        private void DropWeapon()
        {
            #region - Description -
            /*
             * The goal of this look at are current weapon, update ammo variables on the pickup side of the weapon,
             * move the weapon to the dropped weapons container, and finally move the position of the pickupable weapon
             * to where the player is, simulating the experience of "dropping the weapon".
             */
            #endregion

            if (dropWeapon.WasPerformedThisFrame() && weapons.Count > 0)
            {
                int currentIndex = weapons.IndexOf(currentWeapon);
                if (currentWeapon.name == "No Weapon")
                {
                    Debug.Log("Cannot remove hands!");
                    return;
                }
                if (currentIndex == -1)
                {
                    Debug.LogError("Current weapon not found in the inventory list.");
                    return; // Current weapon not in the list
                }

                currentWeapon.SetActive(false);

                Weapon_Ammo ammoComponent = currentWeapon.GetComponent<Weapon_Ammo>();
                Weapon_Main mainWeaponData = currentWeapon.GetComponent<Weapon_Main>();
                Weapon_Pickupable pickupableComponent = mainWeaponData.dropableObject.GetComponent<Weapon_Pickupable>();

                if (ammoComponent != null && pickupableComponent != null)
                {
                    AmmoData currentAmmoData = ammoComponent.GetAmmoData();
                    pickupableComponent.SetAmmoData(currentAmmoData);
                }

                // Update the counters based on the weapon type
                Weapon_WeaponType weaponType = currentWeapon.GetComponent<Weapon_WeaponType>();
                if (weaponType != null)
                {
                    DecrementWeaponCounter(weaponType.currentWeaponType);
                }
                else
                {
                    Debug.LogError("Weapon_WeaponType component not found on the dropped weapon.");
                }

                // Set the dropped weapon as a child of the 'Dropped Weapons' container
                currentWeapon.transform.SetParent(droppedWeaponsContainer.transform);

                // Spawn the pickupable version and set the original weapon reference
                if (mainWeaponData.dropableObject != null)
                {
                    mainWeaponData.dropableObject.transform.localEulerAngles = Vector3.zero;

                    GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");

                    // Define the distance you want the object to spawn in front of the camera
                    float spawnDistance = 1.2f; // Adjust this value as needed for your game

                    // Calculate the intended spawn position in front of the camera
                    Vector3 intendedSpawnPosition = camera.transform.position + camera.transform.forward * spawnDistance;

                    // Perform a raycast to check for collisions along the intended path
                    if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, spawnDistance))
                    {
                        // If a hit is detected, adjust the spawn position to just in front of the hit point to avoid spawning inside the wall
                        intendedSpawnPosition = hit.point - camera.transform.forward * 0.1f; // Adjust this offset as necessary
                    }

                    // Set the position and rotation of the object
                    mainWeaponData.dropableObject.transform.position = intendedSpawnPosition;
                    // Optional: You might want to set the rotation of the object to match the camera or a certain orientation depending on your game

                    // Activate the object
                    mainWeaponData.dropableObject.SetActive(true);

                    // Log the position for debugging purposes
                    Debug.Log("Spawned Object at: " + intendedSpawnPosition);
                }
                else
                {
                    Debug.LogError("DropableObject reference is destroyed or null.");
                }

                if (!droppedWeapons.Contains(currentWeapon))
                {
                    droppedWeapons.Add(currentWeapon);
                }

                // Remove the current weapon from the inventory
                weapons.Remove(currentWeapon);
                currentInventorySize = weapons.Count; // Update inventory size

                // Switch to the next weapon in the list
                int nextWeaponIndex = (currentIndex >= weapons.Count) ? 0 : currentIndex;
                SwitchWeapon(nextWeaponIndex);
            }
        }


        private void SwitchWeapon(int index)
        {
            if (index < 0 || index >= weapons.Count)
            {
                Debug.LogError("Invalid weapon index.");
                return;
            }
            GameObject weaponToSwitchTo = weapons[index];
            if (weaponToSwitchTo == null)
            {
                Debug.LogError("Weapon at index " + index + " is null.");
                return;
            }

            // Disable the current weapon and stop the reload coroutine
            if (currentWeapon.GetComponent<Weapon_BaseWeaponFire>() != null && currentWeapon.GetComponent<Weapon_Main>())
            {
                currentWeapon.GetComponent<Weapon_Main>().weaponIsReloading = false;
                Debug.Log("Weapon Switch Test");
                currentWeapon.GetComponent<Animator>().StopPlayback();
                currentWeapon.GetComponent<Weapon_BaseWeaponFire>().StopReload();
            }
            else if (currentWeapon.GetComponent<Weapon_ShotgunFire>() != null && currentWeapon.GetComponent<Weapon_Main>())
            {
                currentWeapon.GetComponent<Weapon_Main>().weaponIsReloading = false;
                Debug.Log("Weapon Switch Test");
                currentWeapon.GetComponent<Animator>().StopPlayback();
            }
            currentWeapon.SetActive(false);

            // Activate the new weapon
            currentWeapon = weapons[index]; // Update current weapon
            currentWeapon.SetActive(true); // Activate new weapon
        }


        private void InitWeaponSwitchLogic()
        {
            if (stopWeaponSwitch) return;
            int currentIndex = weapons.IndexOf(currentWeapon);
            if (Time.time - lastSwitchTime < switchCooldown) return;

            var weaponSwitchVal = weaponSwitch.ReadValue<Vector2>();
            if (weaponSwitchVal.y != 0)
            {
                OnWeaponSwitch?.Invoke();
                if (weaponSwitchVal.y > 0)
                {
                    int nextWeaponIndex = (currentIndex + 1) % weapons.Count;
                    SwitchWeapon(nextWeaponIndex);
                }
                else if (weaponSwitchVal.y < 0)
                {
                    int prevWeaponIndex = (currentIndex - 1 + weapons.Count) % weapons.Count;
                    SwitchWeapon(prevWeaponIndex);
                }
                lastSwitchTime = Time.time;
            }
            DropWeapon();
        }

        private void Update()
        {
            InitWeaponSwitchLogic();
            currentInventorySize = weapons.Count;
        }
    }
}