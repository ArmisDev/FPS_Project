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
            Vector3 moveValue = new(playerMovement.playerVelocity.x, 0, playerMovement.playerVelocity.z);
            if(moveValue.magnitude < 0.05)
            {
                moveValue = Vector3.zero;
            }
            animator.SetFloat("LocomotionBlend", moveValue.magnitude);
        }
    }
}