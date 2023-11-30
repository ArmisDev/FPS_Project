using System;
using UnityEngine;

namespace Project.Interaction
{
    public class WeaponInteractionEventArgs : EventArgs
    {
        public GameObject Prefab { get; set; }
        public WeaponType WeaponType { get; set; }
        public string WeaponName { get; set; }

        public WeaponInteractionEventArgs(GameObject prefab, WeaponType weaponType, string weaponName)
        {
            Prefab = prefab;
            WeaponType = weaponType;
            WeaponName = weaponName;
        }
    }
}