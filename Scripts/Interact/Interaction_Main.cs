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

        //Weapon
        Weapon_Pickupable weapon_Pickupable;
        private bool destroyObject;

        private void TriggerInteraction(GameObject prefab, WeaponType weaponType, string weaponName)
        {
            Debug.Log(weaponType);
            OnInteraction?.Invoke(this, new InteractionEventArgs(prefab, weaponType, weaponName));
        }

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            interactButton = playerInput.actions["Interaction"];
        }

        void GetWeaponData(RaycastHit hit)
        {
            //This format helps prevent against null cases. But, it can be simplified.
            //You could also just directly cache the object reference, based on the assumption
            //that if an object uses the WeaponPickup tag, it should house the Weapon_Pickupable class.
            hit.collider.gameObject.TryGetComponent(out Weapon_Pickupable weaponDataComponent);
            if (weaponDataComponent != null)
            {
                Weapon_SCR weaponData = weaponDataComponent.scriptableObject;
                TriggerInteraction(weaponData.weaponPrefab, weaponData.weaponType, weaponData.weaponName);
                if(destroyObject)
                {
                    weaponDataComponent.DestroyObject();
                }
            }
        }

        public void DestroyObjectStateSet(bool state)
        {
            destroyObject = state;
        }

        void Update()
        {
            if(Physics.Raycast(raycastFirePoint.transform.position, raycastFirePoint.transform.forward, out hit, raycastRange) && interactButton.WasPerformedThisFrame())
            {
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