using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.AI
{
    public class AIAttackTrigger : MonoBehaviour
    {
        [SerializeField] private float damage;
        private GameObject target;

        private void Awake()
        {
            target = GameObject.FindGameObjectWithTag("Player");
            Debug.Log(name + " has attached " + target.name + " as it's target");
        }

        //Called by animation event
        public void DealAttackDamage()
        {
            target.TryGetComponent(out Player_Health player_Health);
            if (player_Health != null)
            {
                if (player_Health.health > 0)
                {
                    player_Health.TakeDamage(damage);
                }
            }
        }
    }
}