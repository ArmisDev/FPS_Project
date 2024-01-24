using UnityEngine;
using Project.Weapon;
using System;

public class Weapon_Pickupable : MonoBehaviour
{
    //public GameObject originalWeapon; // Reference to the original weapon GameObject
    public Weapon_SCR scriptableObject; // Reference to the associated ScriptableObject
    public GameObject droppableWeapon;
    public int ammoInMag;
    public int ammoInventory;
    public int maxAmmo;
    public string _uniqueID;
    public string UniqueID
    {
        get => _uniqueID;
        set => _uniqueID = value;
    }

    void Awake()
    {
        GenerateUniqueID();
        droppableWeapon = this.gameObject;
    }

    private void GenerateUniqueID()
    {
        if (string.IsNullOrEmpty(_uniqueID))
        {
            _uniqueID = Guid.NewGuid().ToString();
        }
    }

    public void SetAmmoData(AmmoData data)
    {
        ammoInMag = data.ammoInMag;
        ammoInventory = data.ammoInventory;
        //maxAmmo = data.maxAmmo;
        // ... set other ammo fields ...
    }

    public AmmoData GetAmmoData()
    {
        return new AmmoData(ammoInMag, ammoInventory);
    }

    public void SetVisability()
    {
        gameObject.SetActive(false);
    }
}