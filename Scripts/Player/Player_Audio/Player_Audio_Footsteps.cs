using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player.Audio
{
    [RequireComponent(typeof(AudioSource), typeof(Player_Movement))]
    public class Player_Audio_Footsteps : MonoBehaviour
    {
        [SerializeField] private float walkVolume;
        [SerializeField] private float rotateVolume;
        [SerializeField] private float minAudioRotationThreshold = 100f;
        private Quaternion lastPlayerRotation;
        [SerializeField] private TextureSound[] textureSounds; //Reference to our nested class located below.
        [SerializeField] private LayerMask raycastIgnoreLayer;
        [SerializeField] private float groundCheckLength;
        private Player_Movement player_Movement;
        private AudioSource mainAudioSource;
        private readonly float minMoveThreshold = 1.5f;

        private bool isPlayingFootsteps = false;
        private bool isPlayingRotation = false;

        private void Awake()
        {
            mainAudioSource = GetComponent<AudioSource>();
            player_Movement = GetComponent<Player_Movement>();
        }

        private void Start()
        {
            lastPlayerRotation = transform.rotation;
            StartCoroutine(TextureCheck());
        }

        IEnumerator TextureCheck()
        {
            while (true)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckLength, ~raycastIgnoreLayer))
                {
                    if (hit.collider.TryGetComponent(out Renderer renderer))
                    {
                        // Rotation logic
                        Quaternion currentRotation = transform.rotation;
                        float rotationDifference = Quaternion.Angle(lastPlayerRotation, currentRotation);

                        if (rotationDifference >= minAudioRotationThreshold && !isPlayingRotation)
                        {
                            StartCoroutine(PlayAudioOnRotation(renderer));
                            lastPlayerRotation = transform.rotation;
                        }

                        // Movement logic
                        if (player_Movement.playerVelocity.magnitude > minMoveThreshold && !isPlayingFootsteps)
                        {
                            StartCoroutine(PlayerFootstepSoundFromRenderer(renderer));
                        }
                    }
                }
                yield return null;
            }
        }

        //IEnumerator TextureCheck()
        //{
        //    while (true)
        //    {
        //        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckLength, ~raycastIgnoreLayer))
        //        {
        //            Renderer renderer;
        //            if (hit.collider.TryGetComponent(out renderer))
        //            {
        //                // Rotation logic
        //                Quaternion currentRotation = transform.rotation;
        //                float rotationDifference = Quaternion.Angle(lastPlayerRotation, currentRotation);

        //                if (rotationDifference >= minAudioRotationThreshold)
        //                {
        //                    yield return StartCoroutine(PlayAudioOnRotation(renderer));
        //                    lastPlayerRotation = currentRotation; // Update last rotation
        //                }

        //                // Movement logic
        //                if (player_Movement.playerVelocity.magnitude > minMoveThreshold)
        //                {
        //                    yield return StartCoroutine(PlayerFootstepSoundFromRenderer(renderer));
        //                }
        //            }
        //        }
        //        yield return null;
        //    }
        //}

        private IEnumerator PlayerFootstepSoundFromRenderer(Renderer renderer)
        {
            isPlayingFootsteps = true; // Set the flag when coroutine starts

            foreach (TextureSound textureSound in textureSounds)
            {
                if (textureSound.Albedo == renderer.material.GetTexture("_MainTex"))
                {
                    AudioClip clip = GetClipFromTextureSounds(textureSound);
                    mainAudioSource.PlayOneShot(clip, walkVolume);
                    //I am aware the hard code is ass.
                    // Wait for the duration of the footstep sound
                    yield return new WaitForSeconds(player_Movement.playerVelocity.magnitude > 3 ? player_Movement.playerVelocity.magnitude * 0.1f : player_Movement.playerVelocity.magnitude * 0.28f);
                    break; // Exit the loop after playing the sound
                }
            }

            isPlayingFootsteps = false; // Reset the flag after the loop
        }

        private IEnumerator PlayAudioOnRotation(Renderer renderer)
        {
            isPlayingRotation = true;

            foreach (TextureSound textureSound in textureSounds)
            {
                if (textureSound.Albedo == renderer.material.GetTexture("_MainTex"))
                {
                    AudioClip clip = GetRotationClipFromTextureSounds(textureSound);
                    mainAudioSource.PlayOneShot(clip, rotateVolume);
                    //I am aware the hard code is ass.
                    // Waiting logic
                    yield return new WaitForSeconds(player_Movement.playerVelocity.magnitude > 3 ? player_Movement.playerVelocity.magnitude * 0.1f : player_Movement.playerVelocity.magnitude * 0.28f);
                    break; // Break out of the loop once the sound is played
                }
            }

            isPlayingRotation = false; // Reset the flag after the sound is played or if no sound was played
        }

        private AudioClip GetClipFromTextureSounds(TextureSound textureSound)
        {
            int clipIndex = Random.Range(0, textureSound.footStepClips.Length);
            return textureSound.footStepClips[clipIndex];
        }

        private AudioClip GetRotationClipFromTextureSounds(TextureSound textureSound)
        {
            int clipIndex = Random.Range(0, textureSound.rotateSounds.Length);
            return textureSound.rotateSounds[clipIndex];
        }

        [System.Serializable]
        private class TextureSound
        {
            public Texture Albedo;
            public AudioClip[] footStepClips;
            public AudioClip[] rotateSounds;
        }
    }
}