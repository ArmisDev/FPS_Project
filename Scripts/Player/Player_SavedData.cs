using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Player;
using Project.Weapon;

public class Player_SavedData : MonoBehaviour, IDataPersistance
{
    //Player
    private Player_Health playerHealth;
    private Player_Movement playerMovement;
    private Weapon_Inventory weaponInventory;
    [HideInInspector] public Vector3 dataPlayerPosition;

    //Weapons
    private GameObject currentWeapon;
    private string currentWeaponID;
    private List<GameObject> currentWeapons = new List<GameObject>();
    private List<GameObject> droppedWeapons = new List<GameObject>();

    private void Awake()
    {
        //Chache Components
        playerHealth = GetComponent<Player_Health>();
        playerMovement = GetComponent<Player_Movement>();
        weaponInventory = FindObjectOfType<Weapon_Inventory>();
    }

    public void LoadData(GameData data)
    {
        #region - Player -
        //Player Movement
        playerMovement.characterController.enabled = false;
        playerMovement.transform.position = data.playerPos; // or localPosition, depending on your needs
        playerMovement.characterController.enabled = true;
        #endregion

        #region - Weapons -
        //Player Health
        if (data.playerHealth > 0) playerHealth.health = data.playerHealth;
        //Just default back to full health
        else playerHealth.health = 100;

        //Load weapon Logic
        // Clear current inventory and dropped weapons lists

        //!!!Why am I clearing out the list? This is done on save!!!
        weaponInventory.weapons.Clear();
        weaponInventory.droppedWeapons.Clear();

        //First thing we need to do is add weapons to be childed to the weaponInventory and set their visibility to false.
        //Then do the same but for our dropped weapons.

        //After adding everything, we just need to set the visibility of the current weapon to true.

        // Load current weapon
        string currentWeaponId = data.CurrentWeaponID; // assuming GameData has this field
        weaponInventory.currentWeapon = FindWeaponById(currentWeaponId);

        if(data.InventoryWeaponIDs != null)
        {
            // Load weapons in inventory
            foreach (string weaponId in data.InventoryWeaponIDs) // assuming GameData has this field
            {
                GameObject weapon = FindWeaponById(weaponId);
                if (weapon != null)
                {
                    weaponInventory.weapons.Add(weapon);
                    // Additional logic to set weapon properties if needed
                }
            }
        }

        if(data.DroppedWeaponIDs != null)
        {
            // Load dropped weapons
            foreach (string weaponId in data.DroppedWeaponIDs) // assuming GameData has this field
            {
                GameObject weapon = FindWeaponById(weaponId);
                if (weapon != null)
                {
                    weaponInventory.droppedWeapons.Add(weapon);
                    // Additional logic to instantiate/place dropped weapons in the game world
                }
            }
        }
        #endregion
    }

    public void SaveData(ref GameData data)
    {
        #region - Player -
        dataPlayerPosition = data.playerPos;
        data.playerPos = playerMovement.transform.position;
        data.playerHealth = playerHealth.health;
        #endregion

        SaveWeapons(ref data);
    }

    #region - WEAPON LOGIC -
    private void SaveWeapons(ref GameData data)
    {
        //If we dont have a current weapon, then by default we dont have any weapons to save so we should not compute anything further
        if (weaponInventory.currentWeapon == null || weaponInventory.currentWeapon.name == "No Weapon") return;

        //Save weapons from inventory
        //First we need to clear the previous list to grab fresh data
        currentWeapons.Clear();
        droppedWeapons.Clear();

        //Then lets grab a reference to the current weapon object and its ID
        currentWeapon = weaponInventory.currentWeapon;
        Weapon_Main currentWeaponMain = weaponInventory.currentWeapon.GetComponent<Weapon_Main>();
        currentWeaponID = currentWeaponMain.weaponID;
        //Now we need to set the current weapon references in our data component to equal that of our references here
        data.CurrentWeaponID = currentWeaponID;
        data.CurrentWeapon = currentWeapon;

        //Then we create a local array to parse through the to weapon holders
        //Then we add the weapons to our list
        Weapon_Main[] currentWeaponMains = weaponInventory.GetComponentsInChildren<Weapon_Main>();
        Weapon_Main[] droppedWeaponMains = weaponInventory.droppedWeaponsContainer.GetComponentsInChildren<Weapon_Main>();
        
        //We can only have one active weapon so outputting it to an array is dumb.
        //Also the active weapon will be the same as the currentWeapon, so again ...dumb.
        //Why am I creating two weaponMain arrays and not using them???
        
        //Grab activeWeapon gameobjects from inventory (aka: Weapons we have picked up and not dropped.
        List<GameObject> activeWeaponsList = new();
        foreach (Transform child in weaponInventory.gameObject.transform)
        {
            activeWeaponsList.Add(child.gameObject);
        }
        GameObject[] activeWeaponsArray = activeWeaponsList.ToArray();

        //Perform the same logic above, but this time for the dropped weapons
        List<GameObject> droppedWeaponsList = new List<GameObject>();
        foreach (Transform child in weaponInventory.droppedWeaponsContainer.transform)
        {
            droppedWeaponsList.Add(child.gameObject);
        }
        GameObject[] droppedWeaponsArray = droppedWeaponsList.ToArray();

        //Wow I am dumb. This is literally only saving the "active weapons" (which will only be one) and nothing else. Nice :).
        //FIX: All lines involved in saving the "current weapons" need to be deleted. Instead, we need to save all the childed weapons under the weapon inventory.
        // after that, when we load the data back in we can set all gameobjects visibility to false and only set the current weapon to active.
        //
        //Gameobject[] currentWeapons = weaponInventory.GetComponentsInChildren<GameObject>();
        //Gameobject[] droppedWeapons = weaponInventory.droppedWeaponsContainer.GetComponentsInChildren<GameObject>();

        //Possible Solution:
        //foreach(Gameobject weapon in currentWeapons)
        //{
        //    data.CurrentWeapons.Add(weapon);
        //    Debug.Log("Save system added " + weapon.name + " to list to be saved");
        //}
        //if (droppedWeaponsArray != null)
        //{
        //    foreach (GameObject weapon in droppedWeaponsArray)
        //    {
        //        droppedWeapons.Add(weapon);
        //        data.DroppedWeapons.Add(weapon);
        //        Debug.Log("Save system added " + weapon.name + " to list to be saved");
        //    }
        //    AddWeaponIDToData(ref data, droppedWeaponMains, true);
        //}
        
        //Now we want to comb through the array and add each weapon to our list here and the one referenced in our data component.
        //Then we want to call AddWeaponIDToData which will comb through the Weapon_Main components, find their respective IDs
        // and add them to the InventoryWeapon/DroppedWeapon IDs list.
        if (activeWeaponsArray != null)
        {
            foreach (GameObject weapon in activeWeaponsArray)
            {
                currentWeapons.Add(weapon);
                data.CurrentWeapons.Add(weapon);
                Debug.Log("Save system added " + weapon.name + " to list to be saved");
            }
            AddWeaponIDToData(ref data, currentWeaponMains, false);
        }
        if (droppedWeaponsArray != null)
        {
            foreach (GameObject weapon in droppedWeaponsArray)
            {
                droppedWeapons.Add(weapon);
                data.DroppedWeapons.Add(weapon);
                Debug.Log("Save system added " + weapon.name + " to list to be saved");
            }
            AddWeaponIDToData(ref data, droppedWeaponMains, true);
        }
    }
    //Called from Load
    private GameObject FindWeaponById(string weaponId)
    {
        Weapon_Main[] currentWeaponMains = weaponInventory.GetComponentsInChildren<Weapon_Main>();

        // Check if a weapon with this ID already exists
        foreach (Weapon_Main existingWeapon in currentWeaponMains)
        {
            if (existingWeapon.weaponID == weaponId)
            {
                // Return the existing weapon, or decide if it needs to be replaced
                return existingWeapon.gameObject;
            }
        }

        // If no existing weapon was found, instantiate a new one
        foreach (Weapon_Main weaponMain in currentWeaponMains)
        {
            if (weaponMain.weaponID == weaponId)
            {
                return Instantiate(weaponMain.weaponPrefab, weaponInventory.GunsTransform);
            }
        }

        return null;
    }

    //Called from Save
    private void AddWeaponIDToData(ref GameData data, Weapon_Main[] weapon_Mains, bool isDroppedWeapons)
    {
        foreach (Weapon_Main weaponMain in weapon_Mains)
        {
            if (!isDroppedWeapons) data.InventoryWeaponIDs.Add(weaponMain.weaponID);
            else data.DroppedWeaponIDs.Add(weaponMain.weaponID);
        }
    }
    #endregion
}
