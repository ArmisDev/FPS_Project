using System;
using System.Collections;
using UnityEngine;

public class AIDeathHandler : MonoBehaviour
{
    [SerializeField] private float timeBeforeDespawn = 5f;
    public event Action OnDeath;
    [HideInInspector] public bool hasBeenKilled;

    public void HandleAIDeath(BlazeAI blazeAI)
    {
        //OnDeath?.Invoke();
        blazeAI.SetState(BlazeAI.State.death);
        hasBeenKilled = true;
        //Destroy(gameObject, timeBeforeDespawn);
        StartCoroutine(ObjectHideTime());
    }

    IEnumerator ObjectHideTime()
    {
        yield return new WaitForSeconds(timeBeforeDespawn);
        gameObject.SetActive(false);
    }
}
