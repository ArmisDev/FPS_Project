using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main))]
    public class Weapon_ShotgunFire : MonoBehaviour
    {
        [SerializeField] private int pellets;
        [SerializeField] private float spread;

        private Weapon_Main weaponMain;

        private void Awake()
        {
            weaponMain = GetComponent<Weapon_Main>();
            weaponMain.OnFire += ShotgunFire;
        }

        private void OnDestroy()
        {
            weaponMain.OnFire -= ShotgunFire;
        }

        void ShotgunFire()
        {
            for (int i = 0; i < Mathf.Max(1, pellets); i++)
            {
                // Calculate the forward direction
                Vector3 forward = weaponMain.firePoint.transform.forward;

                // Add randomness to the direction
                float randomX = Random.Range(-spread, spread);
                float randomY = Random.Range(-spread, spread);
                Vector3 randomDirection = forward + new Vector3(randomX, randomY, 0);

                // Normalize the direction to maintain consistent length
                randomDirection.Normalize();

                // Calculate the final target point
                Vector3 f_branch = weaponMain.firePoint.transform.position + randomDirection * 1000f;
                Physics.Raycast(weaponMain.firePoint.transform.position, f_branch - weaponMain.firePoint.transform.position, out RaycastHit hit, weaponMain.range, ~weaponMain.rayExclusionLayer);
                Debug.DrawRay(weaponMain.firePoint.transform.position, f_branch - weaponMain.firePoint.transform.position, Color.green, 1f);

                weaponMain.HitRigidBodyCheck(hit);
            }
        }
    }
}