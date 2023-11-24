using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AI_Mover aI_Mover;

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Locomotion", aI_Mover.smoothedSpeed);
    }
}
