using System.Collections;
using UnityEngine;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main))]
    public class Weapon_Recoil : MonoBehaviour
    {
        [SerializeField] private GameObject recoilMover;
        [SerializeField] private float setbackForce = 1f; // The force to return to the start rotation
        [SerializeField] private float recoveryRate = 1f; // The speed at which the weapon recovers from recoil
        private Weapon_Main weapon_Main;
        private Quaternion startRotation;
        private Coroutine resetCoroutine;

        private void Awake()
        {
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
            // Wait for a short delay before starting the recovery
            yield return new WaitForSeconds(0.1f);

            // Use a local variable to control the lerp progress
            float recoverProgress = 0f;

            while (recoverProgress < 1f)
            {
                recoverProgress += Time.deltaTime * recoveryRate;
                recoilMover.transform.localRotation = Quaternion.Lerp(recoilMover.transform.localRotation, startRotation, recoverProgress);
                yield return null;
            }

            // Ensure the final position is the start position
            recoilMover.transform.localRotation = startRotation;
        }
    }
}