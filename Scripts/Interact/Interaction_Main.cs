using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Interaction
{
    [RequireComponent(typeof(PlayerInput))]
    public class Interaction_Main : MonoBehaviour
    {
        [SerializeField] private Transform raycastFirePoint;
        [SerializeField] private float raycastRange;
        private RaycastHit hit;
        public event EventHandler<InteractionEventArgs> OnInteraction;
        private PlayerInput playerInput;
        private InputAction interactButton;

        private void TriggerInteraction(GameObject prefab, WeaponType weaponType, string weaponName)
        {
            OnInteraction?.Invoke(this, new InteractionEventArgs(prefab, weaponType, weaponName));
        }

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            interactButton = playerInput.actions["Interaction"];
        }

        void GetWeaponData(RaycastHit hit)
        {
            Weapon_Pickupable weaponDataComponent = hit.collider.gameObject.GetComponent<Weapon_Pickupable>();
            if (weaponDataComponent != null)
            {
                Weapon_SCR weaponData = weaponDataComponent.scriptableObject;
                WeaponPickUpInteraction(weaponData.weaponPrefab, weaponData.weaponType, weaponData.weaponName);
                weaponDataComponent.DestroyObject();
            }
        }

        void WeaponPickUpInteraction(GameObject weaponPrefab, WeaponType weaponType, string weaponName)
        {
            TriggerInteraction(weaponPrefab, weaponType, weaponName);
        }

        // Update is called once per frame
        void Update()
        {
            if(Physics.Raycast(raycastFirePoint.transform.position, raycastFirePoint.transform.forward, out hit, raycastRange) && interactButton.WasPerformedThisFrame())
            {
                Debug.Log("Input has been pressed");
                switch (hit.collider.tag)
                {
                    case "WeaponPickup":
                        GetWeaponData(hit);
                        break;
                }
            }
        }
    }
}