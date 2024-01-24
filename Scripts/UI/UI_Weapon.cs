using UnityEngine;
using Project.Weapon;
using TMPro;
using UnityEngine.UI;

public class UI_Weapon : MonoBehaviour
{
    [SerializeField] private Weapon_Inventory weaponInventory;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Sprite rifleImage;
    [SerializeField] private Sprite pistolImage;
    [SerializeField] private Sprite shotgunImage;
    [SerializeField] private Image weaponImage; // Use Image component instead of GameObject
    private Weapon_WeaponType currentWeaponType;
    private Weapon_Ammo currentWeaponData;

    void Update()
    {
        // Check if the current weapon is null
        if (weaponInventory.currentWeapon == null)
        {
            weaponImage.gameObject.SetActive(false);
            ammoText.gameObject.SetActive(false);
            return;
        }

        currentWeaponType = weaponInventory.currentWeapon.GetComponent<Weapon_WeaponType>();
        currentWeaponData = weaponInventory.currentWeapon.GetComponent<Weapon_Ammo>();

        // Check if the required components are available
        if (currentWeaponType == null || currentWeaponData == null)
        {
            weaponImage.gameObject.SetActive(false);
            ammoText.gameObject.SetActive(false);
            return;
        }

        // Update UI elements based on the current weapon
        weaponImage.gameObject.SetActive(true);
        ammoText.gameObject.SetActive(true);

        switch (currentWeaponType.currentFireMode)
        {
            case Weapon_WeaponType.WeaponFireModes.automatic:
                weaponImage.sprite = (currentWeaponType.currentweaponCaliber == Weapon_WeaponType.WeaponCaliber.pistol) ? pistolImage : rifleImage;
                break;
            case Weapon_WeaponType.WeaponFireModes.semiautomatic:
                weaponImage.sprite = (currentWeaponType.currentweaponCaliber == Weapon_WeaponType.WeaponCaliber.pistol) ? pistolImage : rifleImage;
                break;
            case Weapon_WeaponType.WeaponFireModes.shotgun:
                weaponImage.sprite = shotgunImage;
                break;
        }

        ammoText.text = currentWeaponData.ammoInMag.ToString() + "/" + currentWeaponData.ammoInventory;
    }
}