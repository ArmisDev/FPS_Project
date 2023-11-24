using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main))]
    public class Weapon_FX : MonoBehaviour
    {
        [Header("Particle FX")]
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private ParticleSystem bulletEject;
        [SerializeField] private GameObject bulletDecal;

        //Components
        private Weapon_Main weapon_Main;

        [Header("Smoke Attributes")]
        [SerializeField] private bool useSmoke;
        [SerializeField] private ParticleSystem smoke;
        [SerializeField] private float smokeToFireThreshold;
        [SerializeField] private float smokeTimerThreshold;

        //Private Smoke Variables
        public int fireIncrement;
        public float smokeTimer;
        private bool smokeCoroutineIsRunning;

        private void Awake()
        {
            weapon_Main = GetComponent<Weapon_Main>();
            weapon_Main.OnFire += PlayFX;
            //weapon_Main.OnFire += SpawnHitDecal;
        }

        private void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            if (weapon_Main != null)
            {
                weapon_Main.OnFire -= PlayFX;
                //weapon_Main.OnFire -= SpawnHitDecal;
            }
        }

        void PlayFX()
        {
            smokeTimer += Time.deltaTime * 10;
            fireIncrement++;
            muzzleFlash.Play();
            bulletEject.Play();
        }

        void PlaySmoke()
        {
            if (!useSmoke) return;

            // Start smoke if fire increment threshold is reached and smoke is not already playing
            if (fireIncrement >= smokeToFireThreshold && !smoke.isPlaying && !smokeCoroutineIsRunning)
            {
                smoke.Play();
                smokeCoroutineIsRunning = true;
            }

            // Continue smoke for a set duration after firing stops
            if (!weapon_Main.weaponIsFiring && smoke.isPlaying && smokeCoroutineIsRunning)
            {
                if (smokeTimer >= smokeTimerThreshold)
                {
                    StartCoroutine(StopSmokeCoroutine());
                }
            }
        }

        IEnumerator StopSmokeCoroutine()
        {
            // Wait for a set time before stopping the smoke
            yield return new WaitForSeconds(smokeTimer * 1.3f);
            smoke.Stop();
            fireIncrement = 0;
            smokeTimer = 0;
            smokeCoroutineIsRunning = false;
        }

        //void SpawnHitDecal()
        //{
        //    Vector3 hitPos = weapon_Main.raycastHit.normal;
        //    Vector3 distanceFromObj = weapon_Main.raycastHit.normal - weapon_Main.raycastHit.transform.position;
        //    Debug.Log(distanceFromObj);
        //    Vector3 offset = weapon_Main.raycastHit.point * 0.01f;
        //    Vector3 spawnPosition = weapon_Main.raycastHit.point + offset;
        //    Quaternion spawnRotation = Quaternion.LookRotation(weapon_Main.raycastHit.normal);
        //    Instantiate(bulletDecal, spawnPosition, spawnRotation);
        //}

        private void Update()
        {
            PlaySmoke();
        }

    }
}