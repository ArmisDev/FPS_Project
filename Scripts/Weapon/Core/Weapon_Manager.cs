using System.Collections.Generic;
using UnityEngine;

public class Weapon_Manager : MonoBehaviour
{
    public static Weapon_Manager Instance { get; set; }

    private Dictionary<string, (int ammoInMag, int maxAmmo, int ammoInventory)> weaponStates;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            weaponStates = new Dictionary<string, (int ammoInMag, int maxAmmo, int ammoInventory)>();
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SaveWeaponState(string uniqueID, int ammoInMag, int maxAmmo, int ammoInventory)
    {
        weaponStates[uniqueID] = (ammoInMag, maxAmmo, ammoInventory);
    }

    public (int ammoInMag, int maxAmmo, int ammoInventory) GetSavedAmmoState(string uniqueID)
    {
        if (weaponStates.TryGetValue(uniqueID, out var state))
        {
            return state;
        }
        return (0, 0, 0); // Default values if the weapon state is not found
    }

    public void RestoreWeaponState(string uniqueID, Weapon_Ammo weaponAmmo)
    {
        if (weaponStates.TryGetValue(uniqueID, out var savedState))
        {
            weaponAmmo.ammoInMag = savedState.ammoInMag;
            weaponAmmo.ammoInventory = savedState.ammoInventory;
            weaponAmmo.maxAmmo = savedState.maxAmmo;
        }
    }

    public bool HasSavedState(string uniqueID)
    {
        return weaponStates.ContainsKey(uniqueID);
    }
}