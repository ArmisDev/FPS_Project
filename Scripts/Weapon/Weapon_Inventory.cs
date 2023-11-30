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
        [SerializeField] private int inventorySize;
        [SerializeField] private int maxPrimary;
        [SerializeField] private int maxSecondary;
        [SerializeField] private int maxSpecial;
        private GameObject currentWeapon;
        private GameObject currentActiveWeapon;
        private GameObject[] weapon_Mains;
        private List<GameObject> weapons = new List<GameObject>();
        private Interaction_Main interaction_Main;
        private float switchCooldown = .3f; // 0.5 seconds cooldown between switches
        private float lastSwitchTime = -1f; // Initialize to -1 to allow immediate switch at start
        public int currentInventorySize;
        [SerializeField] private Transform GunsTransform;
        //Input
        private PlayerInput input;
        private InputAction weaponSwitch;

        //Event
        private event Action OnWeaponSwitch;

        // Count the number of each type of weapon in the inventory
        int currentPrimaryCount = 0;
        int currentSecondaryCount = 0;
        int currentSpecialCount = 0;

        private void Awake()
        {
            //Grab inputs
            input = GetComponent<PlayerInput>();
            weaponSwitch = input.actions["SwitchWeapons"];

            //Grab weapon mains for loop in start
            //weapon_Mains = GetComponentsInChildren<Weapon_Main>();
            weapon_Mains = GameObject.FindGameObjectsWithTag("Weapon");
            interaction_Main = GetComponent<Interaction_Main>();
            interaction_Main.OnInteraction += UpdateInventory;
        }

        private void Start()
        {
            //sets all the objects found in the array into a list
            foreach (GameObject weapon_Main in weapon_Mains)
            {
                GameObject tempObj = weapon_Main.gameObject;
                tempObj.gameObject.SetActive(false);
                weapons.Add(tempObj);
            }

            currentWeapon = weapon_Mains[0].gameObject;
            if(currentWeapon != null)
            {
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

            // Instantiate the new weapon but don't add it to the list yet
            GameObject weaponToAdd = Instantiate(e.Prefab, GunsTransform);
            weaponToAdd.SetActive(false);



            //Adds to our local weapontype counter (above) so we can reference it later.
            switch (e.WeaponType)
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

            Debug.Log(currentPrimaryCount);
            // Check if adding the new weapon would exceed the type limits
            switch (e.WeaponType)
            {
                case WeaponType.primary:
                    if (currentPrimaryCount > maxPrimary)
                    {
                        interaction_Main.DestroyObjectStateSet(false);
                        Destroy(weaponToAdd); // Clean up the instantiated but unused weapon
                        return;
                    }
                    break;
                case WeaponType.secondary:
                    if (currentSecondaryCount > maxSecondary)
                    {
                        interaction_Main.DestroyObjectStateSet(false);
                        Destroy(weaponToAdd);
                        return;
                    }
                    break;
                case WeaponType.special:
                    if (currentSpecialCount > maxSpecial)
                    {
                        interaction_Main.DestroyObjectStateSet(false);
                        Destroy(weaponToAdd);
                        return;
                    }
                    break;
            }

            // Now add the new weapon to the inventory
            weapons.Add(weaponToAdd);
            currentWeapon.SetActive(false);
            currentWeapon = weaponToAdd;
            weaponToAdd.SetActive(true);
            interaction_Main.DestroyObjectStateSet(true);
        }


        private void SwitchWeapon(int index)
        {
            // Disable the current weapon and stop the reload coroutine
            if (currentWeapon.GetComponent<Weapon_Main>() != null)
            {
                currentWeapon.GetComponent<Weapon_Main>().StopReload();
            }
            currentWeapon.SetActive(false);

            // Activate the new weapon
            currentWeapon = weapons[index]; // Update current weapon
            currentWeapon.SetActive(true); // Activate new weapon
        }

        private void InitWeaponSwitchLogic()
        {
            // Check if the cooldown has passed
            if (Time.time - lastSwitchTime < switchCooldown)
            {
                return; // Exit if we're still in the cooldown period
            }

            var weaponSwitchVal = weaponSwitch.ReadValue<Vector2>();

            //Ensure that the input value does not excede 1 or -1
            if(weaponSwitchVal.y > 1)
            {
                weaponSwitchVal.y = 1;
            }

            else if(weaponSwitchVal.y < -1)
            {
                weaponSwitchVal.y = -1;
            }

            // Check if the mouse scroll was used
            if (weaponSwitchVal.y != 0)
            {
                OnWeaponSwitch?.Invoke();
                int currentIndex = weapons.IndexOf(currentWeapon);

                if (weaponSwitchVal.y > 0)
                {
                    // Scroll up
                    int nextWeaponIndex = (currentIndex + 1) % weapons.Count;
                    SwitchWeapon(nextWeaponIndex);
                }
                else if (weaponSwitchVal.y < 0)
                {
                    // Scroll down
                    int prevWeaponIndex = (currentIndex - 1 + weapons.Count) % weapons.Count;
                    SwitchWeapon(prevWeaponIndex);
                }

                // Do not activate the currentWeapon here since SwitchWeapon now handles it

                // Update the last switch time
                lastSwitchTime = Time.time;
            }
        }

        private void Update()
        {
            InitWeaponSwitchLogic();
            currentInventorySize = weapons.Count();
        }


    }
}