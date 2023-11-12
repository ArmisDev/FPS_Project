using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Weapon;

namespace Project.Interaction
{
    public class InteractionEventArgs : EventArgs
    {
        public GameObject Prefab { get; set; }
        public WeaponType WeaponType { get; set; } // Assuming WeaponType is an enum you've defined
        public string WeaponName { get; set; }

        public InteractionEventArgs(GameObject prefab, WeaponType weaponType, string weaponName)
        {
            Prefab = prefab;
            WeaponType = weaponType;
            WeaponName = weaponName;
        }
    }
}