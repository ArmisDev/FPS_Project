using System;
using UnityEngine;

public class WeaponInteractionEventArgs : EventArgs
{
    public GameObject Prefab { get; set; }
    public GameObject DroppableWeapon { get; set; }
    public WeaponType WeaponType { get; set; }
    public string WeaponName { get; set; }
    public string UniqueID { get; set; } // Add this line

    public WeaponInteractionEventArgs(GameObject prefab, GameObject droppableWeapon, WeaponType weaponType, string weaponName, string uniqueID)
    {
        Prefab = prefab;
        DroppableWeapon = droppableWeapon;
        WeaponType = weaponType;
        WeaponName = weaponName;
        UniqueID = uniqueID; // Set the uniqueID
    }
}