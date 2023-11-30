using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player.Audio
{
    [RequireComponent(typeof(AudioSource), typeof(Player_Movement))]
    public class Player_Audio_Footsteps : MonoBehaviour
    {
        [SerializeField] private TextureSound[] textureSounds; //Reference to our nested class located below.
        [SerializeField] private LayerMask raycastIgnoreLayer;
        [SerializeField] private float groundCheckLength;
        private Player_Movement player_Movement;
        private AudioSource audioSource;
        private float minMoveThreshold = 1.5f;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            player_Movement = GetComponent<Player_Movement>();
        }

        private void Start()
        {
            StartCoroutine(TextureCheck());
        }

        IEnumerator TextureCheck()
        {
            while(true)
            {
                if (player_Movement.playerVelocity.magnitude > minMoveThreshold && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckLength, ~raycastIgnoreLayer))
                {
                    if (hit.collider.TryGetComponent(out Renderer renderer))
                    {
                        yield return StartCoroutine(PlayerFootstepSoundFromRenderer(renderer));
                    }

                    ////Debug
                    //Debug.Log("Grabbed Renderer from" + hit.collider.name + " Proof: " + objectRenderer);
                    //Debug.DrawRay(transform.position, Vector3.down * groundCheckLength, Color.green);
                }

                yield return null;
            }
        }

        private IEnumerator PlayerFootstepSoundFromRenderer(Renderer renderer)
        {
            foreach (TextureSound textureSound in textureSounds)
            {
                if (textureSound.Albedo == renderer.material.GetTexture("_MainTex"))
                {
                    AudioClip clip = GetClipFromTextureSounds(textureSound);
                    audioSource.PlayOneShot(clip);
                    //I am aware the hard code is ass.
                    if(player_Movement.playerVelocity.magnitude > 3)
                    {
                        yield return new WaitForSeconds(player_Movement.playerVelocity.magnitude * 0.1f);
                    }
                    else yield return new WaitForSeconds(player_Movement.playerVelocity.magnitude * 0.28f);
                }
            }
        }

        private AudioClip GetClipFromTextureSounds(TextureSound textureSound)
        {
            int clipIndex = Random.Range(0, textureSound.footStepClips.Length);
            return textureSound.footStepClips[clipIndex];
        }

        [System.Serializable]
        private class TextureSound
        {
            public Texture Albedo;
            public AudioClip[] footStepClips;
        }
    }
}