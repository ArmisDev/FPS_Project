using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
    [RequireComponent(typeof(Player_Health), typeof(AudioSource), typeof(Player_Respawn))]
    public class Player_KilledLogic : MonoBehaviour
    {
        private Player_Respawn respawn;
        private Player_Health playerHealth;
        private Player_Movement playerMovement;
        private Animator animator;
        private AudioSource audioSource;
        [SerializeField] private float respawnWaitTime;
        [SerializeField] private AudioClip killedSound;
        [SerializeField] private AudioClip killedMechanicalSound;
        public bool eventHasHappend;

        // Start is called before the first frame update
        void Awake()
        {
            respawn = GetComponent<Player_Respawn>();
            playerHealth = GetComponent<Player_Health>();
            playerMovement = GetComponent<Player_Movement>();
            TryGetComponent(out Animator currentAnimator);

            animator = GetComponent<Animator>();
            animator.enabled = false;

            audioSource = GetComponent<AudioSource>();

            playerHealth.OnKilledEvent += DeathHandleLogic;
        }

        private void OnDestroy()
        {
            playerHealth.OnKilledEvent -= DeathHandleLogic;
        }

        void DeathHandleLogic()
        {
            if(!eventHasHappend)
            {
                //Set move speed and shouldStop logic to move to zero and false
                playerMovement.stopMovement = true;
                playerMovement.playerVelocity = Vector3.zero;
                //Stop look ability
                playerMovement.stopLookRotation = true;

                //If animator is not null then trigger killed animation
                animator.enabled = true;
                animator.SetBool("killed", true);

                //Play sound when player has been killed
                audioSource.PlayOneShot(killedSound);
                audioSource.PlayOneShot(killedMechanicalSound);
                StartCoroutine(RespawnWaitEvent());
                eventHasHappend = true;
            }
        }

        IEnumerator RespawnWaitEvent()
        {
            yield return new WaitForSeconds(respawnWaitTime);
            animator.SetBool("killed", false);
            respawn.Respawn();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}