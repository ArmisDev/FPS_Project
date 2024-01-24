using UnityEngine;
using Project.Weapon;
using System;

public class Weapon_Ammo : MonoBehaviour
{
    public int ammoInMag;
    public int maxAmmo;
    public int ammoInventory;
    public int roundsFired;

    // Methods to save and restore ammo data
    public AmmoData GetAmmoData()
    {
        return new AmmoData(ammoInMag, ammoInventory);
    }

    public void SetAmmoData(AmmoData data)
    {
        ammoInMag = data.ammoInMag;
        //maxAmmo = data.maxAmmo;
        ammoInventory = data.ammoInventory;
    }
}

[Serializable]
public class AmmoData
{
    public int ammoInMag;
    public int maxAmmo;
    public int ammoInventory;

    public AmmoData(int ammoInMag, int ammoInventory)
    {
        this.ammoInMag = ammoInMag;
        //this.maxAmmo = maxAmmo;
        this.ammoInventory = ammoInventory;
    }
}