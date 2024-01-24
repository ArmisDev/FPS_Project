using System;
using UnityEngine;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_WeaponType), typeof(AudioSource))]
    public class WeaponAudio : MonoBehaviour
    {
        [SerializeField] private Weapon_AudioConfigScriptableObject AudioConfig;
        private AudioSource audioSource;

        private void Awake()
        {
            // Ensure there is an AudioSource component and get it
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                Debug.LogError("AudioSource component not found on the object!");
            }
        }

        public void PlayWeaponFireAudio(bool IsLastBullet)
        {
            AudioConfig.PlayShootingClip(audioSource, IsLastBullet);
        }

        public void PlayOutOfAmmo()
        {
            AudioConfig.PlayOutOfAmmo(audioSource);
        }

        public void PlayReloadAudio()
        {
            AudioConfig.PlayReloadClip(audioSource);
        }
    }
}
