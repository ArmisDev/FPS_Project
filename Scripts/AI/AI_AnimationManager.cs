using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Project.AI
{
    [RequireComponent(typeof(AIDeathHandler))]
    public class AI_AnimationManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private AIAgent aiAgent;
        private AIDeathHandler aiDeathHandler;

        private void Awake()
        {
            aiDeathHandler = GetComponent<AIDeathHandler>();
        }

        void PlayFootSteps()
        {

        }

        void AnimMovement()
        {
            //Movement
            animator.SetFloat("VelocityX", aiAgent.smoothedVelocityX);
            animator.SetFloat("VelocityZ", aiAgent.smoothedVelocityZ);
            if (aiAgent.smoothedVelocityX > 0 || aiAgent.smoothedVelocityZ > 0)
            {
                animator.SetBool("isMoving", true);
            }
            else animator.SetBool("isMoving", false);
        }

        //void AnimAttack()
        //{
        //    if (aiAgent.distanceFromTarget < 2f && aiAgent.seesPlayer)
        //    {
        //        animator.SetBool("canAttack", true);
        //    }
        //    else if(aiAgent)
        //}

        void Update()
        {
            if(aiDeathHandler.hasBeenKilled)
            {
                animator.SetBool("hasBeenKilled", true);
                return;
            }

            //AnimAttack();
            AnimMovement();

            animator.SetBool("hasBeenKilled", aiDeathHandler.hasBeenKilled);
        }
    }
}