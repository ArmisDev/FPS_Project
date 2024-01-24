using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AIDeathHandler))]
public class AIAgent : MonoBehaviour
{
    [Header("AI Basic Parameters")]
    public float health;
    public float damage;

    [Header("AI Components")]
    public AIPath aiPath;
    [SerializeField] private AIAudioListener audioListener;
    private AIDeathHandler aiDeathHandler;
    [SerializeField] private AIDestinationSetter destinationSetter;
    [SerializeField] private AIDeathHandler aIDeathHandler;

    [Header("Line of Sight Parameters")]
    [SerializeField] private Transform lineOfSightTransform;
    [SerializeField] private float lookDistance;
    [SerializeField] private LayerMask ignoreLayer;

    //Attack Parameters
    [Header("Attack Parameters")]
    [HideInInspector] public GameObject currentTarget;
    public float distanceFromTarget;
    [HideInInspector] public bool seesPlayer;

    //Speed Calc
    [HideInInspector] public float smoothedVelocityX;
    [HideInInspector] public float smoothedVelocityZ;

    //Search Parameters
    //[SerializeField] private float searchTime;
    private float attackTimer;

    #region - AI State -
    //AI state
    public enum AIState
    {
        patrol,
        guard,
        alert,
        search,
        attack
    }
    private AIState _currentState;
    public AIState CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;
        }
    }
    #endregion

    private void Awake()
    {
        aiDeathHandler = GetComponent<AIDeathHandler>();
        aIDeathHandler.OnDeath += StopMovement;
        //previousPosition = transform.position;
    }

    private void OnDestroy()
    {
        aIDeathHandler.OnDeath -= StopMovement;
    }

    void StateHandler()
    {
        switch (_currentState)
        {
            case AIState.alert:
                break;
            case AIState.search:
                break;
            case AIState.attack:
                break;
        }
    }

    #region - Set Target & Rotation -
    public void MoveToPosition(Transform position)
    {
        destinationSetter.target = position;
    }

    public void StopMovement()
    {
        destinationSetter.target = null;
        aiPath.maxSpeed = 0;
    }

    public void RotateToTarget(Transform targetTransform, float rotationSpeed)
    {
        // Calculate the direction to the target
        Vector3 directionToTarget = targetTransform.position - transform.position;
        directionToTarget.y = 0; // Ignore Y-axis differences

        // Calculate the rotation to the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Rotate only on the Y-axis
        Quaternion currentRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    #endregion

    void CalculateVelocity()
    {
        smoothedVelocityX = Mathf.Abs(aiPath.velocity.x);
        smoothedVelocityZ = Mathf.Abs(aiPath.velocity.z);
    }

    void HandleLineofSight()
    {
        Physics.Raycast(lineOfSightTransform.position, lineOfSightTransform.up, out RaycastHit hit, lookDistance, ~ignoreLayer);
        Debug.DrawRay(lineOfSightTransform.position, lineOfSightTransform.up * lookDistance, Color.red);

        if (hit.collider == null) return;

        if (hit.collider.CompareTag("Player"))
        {
            MoveToPosition(hit.collider.transform);
            seesPlayer = true;
            CurrentState = AIState.attack;

            //Increment the timer as long as we see/are attacking the player
            attackTimer += Time.deltaTime;

            distanceFromTarget = Vector3.Distance(transform.position, hit.collider.transform.position);
        }

        else
        {
            seesPlayer = false;
            attackTimer = 0;
        }
    }

    //void SearchProtocol()
    //{
    //    if (audioListener.CanHear() && !seesPlayer)
    //    {
    //        aiBehaviour.CurrentState = AIBehaviour.AIState.alert;
    //        aiBehaviour.RotateToTarget(audioListener.currentAudioSource.transform, 5.0f);

    //        //If we dont see the player then we reset this timer to zero.
    //        attackTimer = 0;
    //    }
    //}

    private void Update()
    {
        //If we the AI is below zero health and it hasn't been killed yet, kill it.
        //if(health <= 0 && !aiDeathHandler.hasBeenKilled)
        //{
        //    health = 0;
        //    aiDeathHandler.HandleAIDeath(blazeAI);
        //    return;
        //}

        StateHandler();
        CalculateVelocity();
        HandleLineofSight();

        if (destinationSetter.target == null) return;
        currentTarget = destinationSetter.target.gameObject;
    }
}
