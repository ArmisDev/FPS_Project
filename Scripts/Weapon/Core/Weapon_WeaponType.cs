using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapon
{
    public class Weapon_WeaponType: MonoBehaviour
    {
        public enum WeaponFireModes
        {
            automatic,
            semiautomatic,
            shotgun
        }

        public enum WeaponCaliber
        {
            assualt,
            pistol,
            shotgun
        }

        public enum WeaponType
        {
            primary,
            secondary,
            special
        }

        public WeaponFireModes currentFireMode;
        public WeaponCaliber currentweaponCaliber;
        public WeaponType currentWeaponType;
    }
}