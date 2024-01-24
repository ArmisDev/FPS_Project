using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Project.AI
{
    [RequireComponent(typeof(AIDeathHandler))]
    public class AI_Mover : MonoBehaviour
    {
        [SerializeField] private float chaseRange;
        private Vector3 previousPosition;
        public float smoothedSpeed;
        public float smoothingFactor = 0.1f; // Adjust this value to control the smoothing
        private AIDeathHandler aIDeathHandler;
        [SerializeField] AIPath aiPath;

        private void Awake()
        {
            aIDeathHandler = GetComponent<AIDeathHandler>();
        }

        void Start()
        {
            previousPosition = transform.position;
            aIDeathHandler.OnDeath += StopMovement;
        }

        private void OnDestroy()
        {
            aIDeathHandler.OnDeath -= StopMovement;
        }

        //Get speed information of the AI Agent
        float CalculateSpeed()
        {
            // Calculate the instantaneous speed
            float distance = Vector3.Distance(transform.position, previousPosition);
            float instantaneousSpeed = distance / Time.deltaTime;

            // Apply smoothing to the speed value
            smoothedSpeed = Mathf.Lerp(smoothedSpeed, instantaneousSpeed, smoothingFactor * Time.deltaTime);

            // Update previousPosition for the next frame
            previousPosition = transform.position;

            // Optionally, output the smoothed speed to the console
            return smoothedSpeed;
        }

        //Stop Movement
        //Called by AIDeathHandler rn
        void StopMovement()
        {
            smoothedSpeed = 0;
            aiPath.canMove = false;
        }

        private void Update()
        {
            CalculateSpeed();
        }
    }
}