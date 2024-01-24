using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Player;

namespace Project.Weapon
{
    [RequireComponent(typeof(PlayerInput))]
    public class Weapon_Main : MonoBehaviour
    {
        //private Player_Movement player_Movement;
        [HideInInspector] public GameObject firePoint;
        [HideInInspector] public PlayerInput input;
        public string weaponID;
        [HideInInspector] public bool stopWeaponLogic = false;
        public GameObject weaponPrefab;
        [HideInInspector] public GameObject dropableObject; //This will be used when drop weapon is called so we have a reference of what weapon should be dropped.
        [HideInInspector] public bool canSwitchFireModes;
        [HideInInspector] public bool weaponIsFiring;
        [HideInInspector] public bool weaponIsReloading;
        [HideInInspector] public bool allowedToFire; //Set by operator in code (see ShotgunReset coroutine for example).
                                                     //Differs from canFire() because this is set by operator.
                                                     //private bool isAiming;
                                                     //[SerializeField] private bool isOnlySemiAutomatic;

        [Header("Weapon Recoil")]
        public float recoilAmount; //Public so recoil script can access

        [Header("Weapon Fire")]
        public float range;
        public float damage;
        public float fireRate;
        //public RaycastHit raycastHit;
        [HideInInspector] public float timeSinceFire;
        public LayerMask rayExclusionLayer;

        [Header("Weapon Ammo")]
        public float reloadTime;
        public int ammoCount;
        public int maxAmmo;
        public Coroutine reloadCoroutine;

        [Header("Input Logic")]
        [Tooltip("Tolerence on when isFiring should be set to false")]
        public float recoveryTimeTolerance; // Tolerence on when isFiring should be set to false
        [HideInInspector] public InputAction fireSemi;
        [HideInInspector] public InputAction fireAuto;
        [HideInInspector] public InputAction reload;
        [HideInInspector] public InputAction fireModeSwitch;
        [HideInInspector] public float recoveryTimeThreshold; // The maximum amount of time allowed after the last shot before setting "weaponIsFiring" to false.

        //Private New Float Values
        [HideInInspector] public float newFireAuto;
        [HideInInspector] public float newFireSemi;
        [HideInInspector] public float newFireModeSwitch;

        private void Awake()
        {
            firePoint = GameObject.FindGameObjectWithTag("MainCamera");
            input = GetComponent<PlayerInput>();
            fireSemi = input.actions["Fire_Semi"];
            fireAuto = input.actions["Fire_Auto"];
            reload = input.actions["Reload"];
            fireModeSwitch = input.actions["FireModeSwitch"];

            recoveryTimeTolerance = recoveryTimeThreshold;
            allowedToFire = true;
        }
    }
}