using System.Collections;
using UnityEngine;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main), typeof(Weapon_WeaponType))]
    public class Weapon_Recoil : MonoBehaviour
    {
        private GameObject recoilMover;
        [SerializeField] private float recoveryRate = 1f; // The speed at which the weapon recovers from recoil
        
        private Quaternion startRotation;
        private Coroutine resetCoroutine;

        //Components
        private Weapon_Main weaponMain;
        private Weapon_WeaponType weaponType;
        private Weapon_BaseWeaponFire baseFire;
        private Weapon_ShotgunFire shotgunFire;

        private void Awake()
        {
            baseFire = GetComponent<Weapon_BaseWeaponFire>();
            weaponMain = GetComponent<Weapon_Main>();
            weaponType = GetComponent<Weapon_WeaponType>();
            recoilMover = GameObject.FindGameObjectWithTag("RecoilMover");

            if (weaponType.currentFireMode == Weapon_WeaponType.WeaponFireModes.shotgun)
            {
                shotgunFire = GetComponent<Weapon_ShotgunFire>();
                shotgunFire.OnFire += GenerateRecoil;
                return;
            }
            else
            {
                baseFire = GetComponent<Weapon_BaseWeaponFire>();
                baseFire.OnFire += GenerateRecoil;
            }
        }

        private void Start()
        {
            startRotation = recoilMover.transform.localRotation;
        }

        private void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            if (weaponType.currentFireMode == Weapon_WeaponType.WeaponFireModes.shotgun)
            {
                shotgunFire.OnFire -= GenerateRecoil;
            }
            else
            {
                baseFire.OnFire -= GenerateRecoil;
            }
        }

        public void GenerateRecoil()
        {
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }

            // Apply immediate recoil effect
            Quaternion recoilRotation = Quaternion.Euler(-weaponMain.recoilAmount, Random.Range(-weaponMain.recoilAmount, weaponMain.recoilAmount), 0);
            recoilMover.transform.localRotation *= recoilRotation;

            // Start the reset coroutine
            resetCoroutine = StartCoroutine(ResetWeaponRotation());
        }

        private IEnumerator ResetWeaponRotation()
        {
            yield return new WaitForSeconds(0.1f); // Wait before starting recovery

            float recoverProgress = 0.01f; // Start from a small positive value

            while (recoverProgress < 1f)
            {
                recoverProgress += Time.deltaTime * recoveryRate;
                recoverProgress = Mathf.Clamp(recoverProgress, 0.01f, 1f);

                if (recoverProgress < Mathf.Epsilon) // Skip Lerp if recoverProgress is too small
                {
                    continue;
                }

                recoilMover.transform.localRotation = Quaternion.Lerp(recoilMover.transform.localRotation, startRotation, recoverProgress);
                yield return null;
            }

            recoilMover.transform.localRotation = startRotation; // Final reset to ensure exact original rotation
        }


        //private bool IsRotationValid(Quaternion rotation)
        //{
        //    // Check for NaN values in the quaternion
        //    return !float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w);
        //}
    }
}