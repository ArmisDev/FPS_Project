using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AIDeathHandler), typeof(BlazeAI))]
public class AI_Health : MonoBehaviour
{
    public float health;
    //[SerializeField] GameObject aiHead;
    //[SerializeField] ParticleSystem bloodSplatter;
    private AIDeathHandler aIDeathHandler;
    private BlazeAI blazeAI;
    //[SerializeField] private List<GameObject> bodyparts = new List<GameObject>();

    private void Awake()
    {
        blazeAI = GetComponent<BlazeAI>();
        //bloodSplatter.Stop();
        aIDeathHandler = GetComponent<AIDeathHandler>();
    }

    public void ApplyDamage(float damge)
    {
        health -= damge;
    }

    public void CheckForBodyParts(RaycastHit hit)
    {
        Debug.Log(hit.collider.name);
        if (hit.collider.CompareTag("AI/BodyPart"))
        {
            hit.collider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(health <= 0)
        {
            //aiHead.SetActive(false);
            //bloodSplatter.Play();
            health = 0;
            blazeAI.Death();
            aIDeathHandler.HandleAIDeath(blazeAI);
            return;
        }
    }
}
