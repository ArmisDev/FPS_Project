using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Scriptable", menuName = "ScriptableObject/Weapon", order = 0)]
public class Weapon_SCR : ScriptableObject
{
    [Header("Type")]
    public string weaponName;
    public WeaponType weaponType;
    public GameObject weaponPrefab;
}
