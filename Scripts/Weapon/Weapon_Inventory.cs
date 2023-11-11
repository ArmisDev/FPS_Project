using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Interaction;

namespace Project.Weapon
{
    [RequireComponent(typeof(PlayerInput))]
    public class Weapon_Inventory : MonoBehaviour
    {
        [Header("Inventory Parameters")]
        [SerializeField] private int inventorySize;
        [SerializeField] private int maxPrimary;
        [SerializeField] private int maxSecondary;
        [SerializeField] private int maxSpecial;
        private GameObject currentWeapon;
        private GameObject[] weapon_Mains;
        private List<GameObject> weapons = new List<GameObject>();

        private float switchCooldown = 1f; // 0.5 seconds cooldown between switches
        private float lastSwitchTime = -1f; // Initialize to -1 to allow immediate switch at start

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
            currentWeapon.SetActive(true);
        }

        public void UpdateInventory(GameObject gameObject)
        {
            //Check if inventory is full
            //If not, add gameobject
            //If so, return.

            //Maybe try instantiating object in inventory??

        }

        private void SwitchWeapon(int index)
        {
            // Disable the current weapon
            // And stop the reload coroutine (if happening) to ensure that it does not break between switch.
            if(currentWeapon.GetComponent<Weapon_Main>() != null)
            {
                currentWeapon.GetComponent<Weapon_Main>().StopReload();
            }
            currentWeapon.SetActive(false);

            // Activate the new weapon
            currentWeapon = weapons[index];
            currentWeapon.SetActive(true);
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
                // Find the index of the currently active weapon
                int currentIndex = weapons.IndexOf(currentWeapon);

                // Disable the current weapon
                currentWeapon.SetActive(false);

                if (weaponSwitchVal.y > 0)
                {
                    // Scroll up
                    int nextWeaponIndex = (currentIndex + 1) % weapons.Count;
                    currentWeapon = weapons[nextWeaponIndex];
                    SwitchWeapon(nextWeaponIndex);
                }
                else if (weaponSwitchVal.y < 0)
                {
                    // Scroll down
                    int prevWeaponIndex = (currentIndex - 1 + weapons.Count) % weapons.Count;
                    currentWeapon = weapons[prevWeaponIndex];
                    SwitchWeapon(prevWeaponIndex);
                }

                // Activate the new current weapon
                currentWeapon.SetActive(true);

                // Update the last switch time
                lastSwitchTime = Time.time;
            }
        }

        private void Update()
        {
            InitWeaponSwitchLogic();
        }


    }
}