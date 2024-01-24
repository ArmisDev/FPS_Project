using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapon
{
    [RequireComponent(typeof(Weapon_Main))]
    public class Weapon_HitCheck : MonoBehaviour
    {
        [SerializeField] private float hitTime = 2.5f;
        private Weapon_Main weaponMain;
        private AI_Health aiHealth;
        private BlazeAI blazeAI;
        BlazeAI.State currentState;
        private bool coroutineIsRunning;

        private void Awake()
        {
            weaponMain = GetComponent<Weapon_Main>();
        }

        public void RayHitCheck(RaycastHit hit)
        {
            switch (hit.collider.tag)
            {
                case "Enemy":
                    EnemyHitCheck(hit);
                    break;
                case "WeaponPickup":
                    HitRigidBodyCheck(hit);
                    break;
                case "Hittable":
                    Debug.Log("Pew! " + hit.collider.name + " will move around, once the code at this section is complete!");
                    HitRigidBodyCheck(hit);
                    break;
                case "Interactable":
                    Debug.Log("Pew! weapon hit " + hit.collider.name);
                    HitRigidBodyCheck(hit);
                    break;
            }
        }

        private void HitRigidBodyCheck(RaycastHit hit)
        {
            Debug.Log(hit.collider.name);
            hit.collider.TryGetComponent(out Rigidbody objectRigidbody);
            if (objectRigidbody == null) return;
            hit.rigidbody.AddForce(-hit.normal * weaponMain.damage, ForceMode.Impulse);
        }

        bool HealthCheck()
        {
            return aiHealth != null && aiHealth.health > 0;
        }

        private void EnemyHitCheck(RaycastHit hit)
        {
            hit.collider.TryGetComponent(out AI_Health aiHealth);
            hit.collider.TryGetComponent(out BlazeAI blazeAI);
            if (aiHealth == null) return;
            if (blazeAI == null) return;
            this.aiHealth = aiHealth;
            this.blazeAI = blazeAI;
            //float hitWaitTime = blazeAI.hitCooldown;
            aiHealth.ApplyDamage(weaponMain.damage);

            currentState = blazeAI.state;

            if (HealthCheck() && !coroutineIsRunning)
            {
                StartCoroutine(HitWaitTime(currentState, blazeAI, hitTime));
            }
        }

        IEnumerator HitWaitTime(BlazeAI.State previousState, BlazeAI blazeAIInStance, float hitTime)
        {
            coroutineIsRunning = true;
            blazeAIInStance.Hit(gameObject, true);
            yield return new WaitForSeconds(hitTime);

            if (blazeAIInStance.state != BlazeAI.State.death) // Replace with your killed state
            {
                blazeAIInStance.SetState(previousState);
            }

            coroutineIsRunning = false;
        }

        private void Update()
        {
            if (blazeAI != null && !HealthCheck() && coroutineIsRunning)
            {
                StopCoroutine(HitWaitTime(currentState, blazeAI, hitTime));
            }
        }

    }
}