using System.Collections;
using UnityEngine;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main))]
    public class Weapon_Recoil : MonoBehaviour
    {
        private GameObject recoilMover;
        [SerializeField] private float setbackForce = 1f; // The force to return to the start rotation
        [SerializeField] private float recoveryRate = 1f; // The speed at which the weapon recovers from recoil
        private Weapon_Main weapon_Main;
        private Quaternion startRotation;
        private Coroutine resetCoroutine;

        private void Awake()
        {
            recoilMover = GameObject.FindGameObjectWithTag("RecoilMover");
            weapon_Main = GetComponent<Weapon_Main>();
            if (weapon_Main != null)
            {
                weapon_Main.OnFire += GenerateRecoil;
            }
            else
            {
                Debug.LogError("Weapon_Main reference not set on WeaponAudio.");
            }
        }

        private void Start()
        {
            startRotation = recoilMover.transform.localRotation;
        }

        private void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            if (weapon_Main != null)
            {
                weapon_Main.OnFire -= GenerateRecoil;
            }
        }

        public void GenerateRecoil()
        {
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }

            // Apply immediate recoil effect
            Quaternion recoilRotation = Quaternion.Euler(-weapon_Main.recoilAmount, Random.Range(-weapon_Main.recoilAmount, weapon_Main.recoilAmount), 0);
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


        private bool IsRotationValid(Quaternion rotation)
        {
            // Check for NaN values in the quaternion
            return !float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w);
        }
    }
}