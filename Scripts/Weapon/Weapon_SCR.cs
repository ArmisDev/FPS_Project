using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Scriptable", menuName = "ScriptableObject/Weapon", order = 0)]
public class Weapon_SCR : ScriptableObject
{
    [Header("Type")]
    public string weaponName;
    public enum WeaponType
    {
        primary,
        secondary,
        special
    }
    public WeaponType weaponType;
    public GameObject weaponPrefab;
    [Header("Fire Attributes")]
    public float range;
    public float damage;
    public float fireRate;
    [Header("Reload Attributes")]
    public float reloadTime;
    public float ammoCount;
    public float maxAmmo;
}
