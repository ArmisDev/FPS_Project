using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Player;

namespace Project.Weapon
{
    [RequireComponent(typeof(Animator))]
    public class Weapon_NoWeaponAniamtion : MonoBehaviour
    {
        private Animator animator;
        [SerializeField] private Player_Movement playerMovement;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            animator.SetFloat("LocomotionBlend", playerMovement.playerVelocity.magnitude);
        }
    }
}