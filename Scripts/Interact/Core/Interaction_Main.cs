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
        public Transform raycastFirePoint;
        public float raycastRange;
        public LayerMask ignoreLayer;
        private RaycastHit hit;
        public event EventHandler<WeaponInteractionEventArgs> OnInteraction;
        private PlayerInput playerInput;
        private InputAction interactButton;
        private Weapon_Pickupable currentPickupable;

        //Weapon
        Weapon_Pickupable weapon_Pickupable;
        public bool destroyObject;

        //Interact Event
        public event Action OnInteract;

        private void TriggerInteraction(GameObject prefab, GameObject droppableWeapon, WeaponType weaponType, string weaponName, string uniqueID)
        {
            OnInteraction?.Invoke(this, new WeaponInteractionEventArgs(prefab, droppableWeapon, weaponType, weaponName, uniqueID));
        }

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            interactButton = playerInput.actions["Interaction"];
        }

        void GetWeaponData(RaycastHit hit)
        {
            // Check if the hit object has a Weapon_Pickupable component
            if (hit.collider.gameObject.TryGetComponent(out Weapon_Pickupable weaponDataComponent))
            {
                // Retrieve the data from the scriptableObject
                Weapon_SCR weaponData = weaponDataComponent.scriptableObject;
                currentPickupable = weaponDataComponent;
                // Trigger the interaction event with all necessary data, including UniqueID
                TriggerInteraction(weaponData.weaponPrefab, weaponDataComponent.droppableWeapon, weaponData.weaponType, weaponData.weaponName, weaponDataComponent.UniqueID);
            }
        }

        void SetVisabilty()
        {
            if (currentPickupable != null)
            {
                currentPickupable.SetVisability();
            }
        }


        public void DestroyObjectStateSet(bool state)
        {
            destroyObject = state;
        }

        void Update()
        {
            if(Physics.Raycast(raycastFirePoint.transform.position, raycastFirePoint.transform.forward, out hit, raycastRange, ~ignoreLayer) && interactButton.WasPerformedThisFrame())
            {
                Debug.DrawRay(raycastFirePoint.transform.position, raycastFirePoint.transform.forward * raycastRange, Color.red);
                switch (hit.collider.tag)
                {
                    case "WeaponPickup":
                        GetWeaponData(hit);
                        SetVisabilty();
                        break;
                    case "Interactable":
                        hit.collider.TryGetComponent(out InteractableObject interactableObject);
                        if(interactableObject != null)
                        {
                            interactableObject.CallInteractionEvent(this.gameObject, hit);
                        }
                        break;
                }
            }
        }
    }
}