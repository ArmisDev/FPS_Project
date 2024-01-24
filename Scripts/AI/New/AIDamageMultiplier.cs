using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AIDamageMultiplier : MonoBehaviour
{
    [SerializeField] private int damageMult;
    [SerializeField] private AIAgent aIAgent;
    [HideInInspector] public Collider collider;

    private void Awake()
    {
        if(damageMult <= 0)
        {
            damageMult = 1;
        }
        collider = GetComponent<Collider>();
    }

    public void ApplyDamage(float damage)
    {
        aIAgent.health -= damage * damageMult;
    }
}
