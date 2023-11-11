using System;
using UnityEngine;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main), typeof(AudioSource))]
    public class WeaponAudio : MonoBehaviour
    {
        private Weapon_Main weaponMain; // Assign this in the inspector
        [SerializeField] private AudioClip[] weaponFireClips; // Array of firing sounds
        [SerializeField] private AudioClip reloadClip; // Reload sound
        private AudioSource audioSource;

        private void Awake()
        {
            // Ensure there is an AudioSource component and get it
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component not found on the object!");
                return;
            }

            weaponMain = GetComponent<Weapon_Main>();
            // Subscribe to the Weapon_Main events if weaponMain is assigned
            if (weaponMain != null)
            {
                weaponMain.OnFire += PlayWeaponFireAudio;
                weaponMain.OnReload += PlayReloadAudio;
            }
            else
            {
                Debug.LogError("Weapon_Main reference not set on WeaponAudio.");
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            if (weaponMain != null)
            {
                weaponMain.OnFire -= PlayWeaponFireAudio;
                weaponMain.OnReload -= PlayReloadAudio;
            }
        }

        private void PlayWeaponFireAudio()
        {
            if (weaponFireClips.Length > 0)
            {
                AudioClip clipToPlay = weaponFireClips[UnityEngine.Random.Range(0, weaponFireClips.Length)];
                audioSource.PlayOneShot(clipToPlay);
            }
            else
            {
                Debug.LogWarning("Weapon fire clips array is empty!");
            }
        }

        private void PlayReloadAudio()
        {
            if (reloadClip != null)
            {
                audioSource.PlayOneShot(reloadClip);
            }
            else
            {
                Debug.LogWarning("Reload clip is not assigned!");
            }
        }
    }
}
