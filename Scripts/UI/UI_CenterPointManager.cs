using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Interaction;
using Project.Weapon;

public class UI_CenterPointManager : MonoBehaviour
{
    [SerializeField] private Sprite crosshair;
    [SerializeField] private TextMeshProUGUI objectname;
    [SerializeField] private Sprite interaction;
    [SerializeField] private Image image;
    [SerializeField] private Interaction_Main interactionMain;

    void Start()
    {
        // Make sure 'image' is properly assigned in the Inspector
        if (image == null)
        {
            Debug.LogError("Image component not assigned in the Inspector.");
        }
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(interactionMain.raycastFirePoint.position, interactionMain.raycastFirePoint.forward, out hit, interactionMain.raycastRange, ~interactionMain.ignoreLayer))
        {
            switch (hit.collider.tag)
            {
                case "Interactable":
                    image.sprite = interaction;
                    break;
                case "WeaponPickup":
                    hit.collider.TryGetComponent(out Weapon_Pickupable pickupable);
                    image.sprite = interaction;
                    objectname.gameObject.SetActive(true);
                    if(pickupable != null)
                    {
                        Weapon_SCR weaponSCR = pickupable.scriptableObject;
                        WeaponType currentWeaponType = weaponSCR.weaponType;
                        objectname.text = hit.collider.name.ToString() + "\n" + "(" + currentWeaponType.ToString() + ")";
                    }
                    else objectname.text = hit.collider.name.ToString();
                    break;
                default:
                    objectname.gameObject.SetActive(false);
                    image.sprite = crosshair;
                    break;
            }
        }
        else
        {
            // Handle the case when the raycast doesn't hit anything
            objectname.gameObject.SetActive(false);
            image.sprite = crosshair;
        }
    }
}