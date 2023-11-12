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
        private float switchCooldown = 1f; // 0.5 seconds cooldown between switches
        private float lastSwitchTime = -1f; // Initialize to -1 to allow immediate switch at start
        public int currentInventorySize;
        [SerializeField] private Transform GunsTransform;
        //Input
        private PlayerInput input;
        private InputAction weaponSwitch;

        //Event
        private event Action OnWeaponSwitch;

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
            //interaction_Main.OnInteraction -= UpdateInventory;
        }

        private void UpdateInventory(object sender, InteractionEventArgs e)
        {
            // Check if inventory is full
            if (weapons.Count >= inventorySize)
            {
                Debug.Log("Inventory is full");
                return;
            }

            // Check weapon type limits
            int weaponTypeCount = weapons.Count(weapon =>
            {
                var pickupable = weapon.GetComponent<Weapon_Pickupable>();
                return pickupable != null && pickupable.scriptableObject.weaponType == e.WeaponType;
            });

            switch (e.WeaponType)
            {
                case WeaponType.primary:
                    if (weaponTypeCount >= maxPrimary) return;
                    break;
                case WeaponType.secondary:
                    if (weaponTypeCount >= maxSecondary) return;
                    break;
                case WeaponType.special:
                    if (weaponTypeCount >= maxSpecial) return;
                    break;
            }

            // Add the weapon to the inventory
            //currentWeapon.SetActive(false);
            GameObject weaponToAdd = Instantiate(e.Prefab, GunsTransform);
            //Makes sure that are weapon is hidden before it gets set to the proper transform values.
            weaponToAdd.SetActive(false);
            //Sets currentWeapon (which in this case, is now the old weapon) to be hidden
            //Then, sets the new weapon (AKA the weapon we just picked up) to be shown.
            currentWeapon.SetActive(false);
            weapons.Add(weaponToAdd);

            if(currentWeapon != weaponToAdd)
            {
                currentWeapon = weaponToAdd;
                weaponToAdd.SetActive(true);
            }
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