using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.AI
{
    public class AI_AttackPassthrough : MonoBehaviour
    {
        private AIAttackTrigger attackTrigger;
        private bool canHitPlayer;

        private void Awake()
        {
            attackTrigger = GetComponentInChildren<AIAttackTrigger>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                canHitPlayer = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) canHitPlayer = false;
        }

        public void InitiateHit()
        {
            if(canHitPlayer)
            {
                attackTrigger.DealAttackDamage();
            }
        }
    }
}