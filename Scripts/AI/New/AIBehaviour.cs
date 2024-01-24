using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIBehaviour : MonoBehaviour
{
    //public enum AIState
    //{
    //    patrol,
    //    guard,
    //    alert,
    //    search,
    //    attack
    //}
    //private AIState _currentState;
    //public AIState CurrentState
    //{
    //    get => _currentState;
    //    set
    //    {
    //        _currentState = value;
    //    }
    //}

    //[SerializeField] private AIPath aiPath;
    //[SerializeField] private AIDestinationSetter destinationSetter;
    //[SerializeField] private AIDeathHandler aIDeathHandler;

    //public GameObject currentTarget;

    ////[Header("Search Parameters")]
    ////[SerializeField] private float searchTime;

    //private void Awake()
    //{
    //    //Get Components
    //    //audioListener = GetComponent<AIAudioListener>();
    //    aIDeathHandler.OnDeath += StopMovement;
    //}

    //private void OnDestroy()
    //{
    //    aIDeathHandler.OnDeath -= StopMovement;
    //}

    ////void SearchProtocol()
    ////{
    ////    if (audioListener.CanHear())
    ////    {
    ////        CurrentState = AIState.search;
    ////    }
    ////}

    //void StateHandler()
    //{
    //    switch(_currentState)
    //    {
    //        case AIState.alert:
    //            break;
    //        case AIState.search:
    //            break;
    //        case AIState.attack:
    //            break;
    //    }
    //}

    //public void MoveToPosition(Transform position)
    //{
    //    destinationSetter.target = position;
    //}

    //public void StopMovement()
    //{
    //    destinationSetter.target = null;
    //}

    //public void RotateToTarget(Transform targetTransform, float rotationSpeed)
    //{
    //    // Calculate the direction to the target
    //    Vector3 directionToTarget = targetTransform.position - transform.position;
    //    directionToTarget.y = 0; // Ignore Y-axis differences

    //    // Calculate the rotation to the target
    //    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

    //    // Rotate only on the Y-axis
    //    Quaternion currentRotation = transform.rotation;
    //    targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

    //    // Smoothly rotate towards the target rotation
    //    transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
    //}

    //void Update()
    //{
    //    StateHandler();

    //    if (destinationSetter.target == null) return;
    //    currentTarget = destinationSetter.target.gameObject;
    //}
}
