using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AIAudioListener : MonoBehaviour
{
    private SphereCollider hearingCollider;
    [SerializeField] private float hearingRadius;
    [SerializeField] private float hearingThreshold;
    private GameObject objectToListenTo;
    private float percievedLoudness;
    private List<AudioSource> audioSources = new List<AudioSource>();

    [HideInInspector] public Transform audioTransform;
    [SerializeField] private GameObject audioInstanciate;

    [HideInInspector] public AudioSource currentAudioSource; //equal to the current audioSource inside the foreach loop
    public float spawnTimer = 0f;
    private bool isSpawning = false;

    private void Awake()
    {
        hearingCollider = GetComponent<SphereCollider>();
        hearingCollider.radius = hearingRadius;
        hearingCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectToListenTo = other.gameObject;
            audioSources.AddRange(other.GetComponentsInChildren<AudioSource>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (objectToListenTo == null) return;

        // Get all AudioSource components from the colliding object
        AudioSource[] tempAudioSources = other.GetComponentsInChildren<AudioSource>();

        // Check each temp audio source to see if it's new
        foreach (AudioSource tempAudio in tempAudioSources)
        {
            // If the audio source is not already in the list, add it
            if (!audioSources.Contains(tempAudio))
            {
                audioSources.Add(tempAudio);
            }
        }

        float sqrDist = Vector3.SqrMagnitude(transform.position - objectToListenTo.transform.position);

        bool heardSomething = false;
        foreach (AudioSource audio in audioSources)
        {
            if (audio != null && audio.isPlaying)
            {
                currentAudioSource = audio;
                percievedLoudness = audio.volume / sqrDist;
                heardSomething = true;
                break;
            }
        }

        if (!heardSomething)
        {
            percievedLoudness = 0;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objectToListenTo)
        {
            audioSources.Clear();
            objectToListenTo = null;
            percievedLoudness = 0;
        }
    }

    public bool CanHear()
    {
        return percievedLoudness > hearingThreshold;
    }

    //private void Update()
    //{
    //    if (CanHear())
    //    {
    //        if (!isSpawning)
    //        {
    //            spawnTimer = 10f;
    //            isSpawning = true;
    //        }
    //    }

    //    if (isSpawning)
    //    {
    //        spawnTimer -= Time.deltaTime;
    //        if (spawnTimer <= 0f)
    //        {
    //            SpawnAudioPoint(currentAudioSource.gameObject);
    //            isSpawning = false;
    //        }
    //    }
    //    Debug.Log(CanHear());
    //}

    //private void SpawnAudioPoint(GameObject gameObjectAudioClip)
    //{
    //    Transform audioLocation = gameObjectAudioClip.transform;
    //    Instantiate(audioInstanciate, audioLocation.position, audioLocation.rotation);
    //    audioTransform = audioInstanciate.transform;
    //    Debug.Log(audioLocation.position);
    //}
}