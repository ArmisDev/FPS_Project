using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Events
{
    [RequireComponent(typeof(BoxCollider))]
    public class Events_AI_Spawner : MonoBehaviour
    {
        //Contain prefab reference of AI to instanciate
        [SerializeField] private GameObject ai;
        //Reference for spawn zone collider.
        [SerializeField] private BoxCollider spawnZone;
        [SerializeField] int spawnAmount;
        [SerializeField] private Events_Trigger trigger;

        private void Awake()
        {
            if (spawnZone == null)
            {
                Debug.LogWarning("Please Add BoxCollider to SpawnZone!!");
            }

            trigger.OnEnter += SpawnAI;
        }

        private void OnDestroy()
        {
            trigger.OnEnter -= SpawnAI;
        }

        void SpawnAI()
        {
            //float spawnZoneArea = spawnZone.size.x * spawnZone.size.y * spawnZone.size.z;
            //List<Vector3> spawnPoints = new List<Vector3>();

            for (int i = 0; i < spawnAmount; i++)
            {
                float randomX = Random.Range(-spawnZone.size.x / 2f, spawnZone.size.x / 2f);
                float randomY = Random.Range(-spawnZone.size.y / 2f, spawnZone.size.y / 2f);
                float randomZ = Random.Range(-spawnZone.size.z / 2f, spawnZone.size.z / 2f);

                Vector3 spawnPoint = spawnZone.transform.position + new Vector3(randomX, randomY, randomZ);
                Instantiate(ai, spawnPoint, ai.transform.rotation);
                Debug.Log("Spawned" + ai.name);
            }
        }

        //Create method that listens for event
        //Once the event has been invoked, we need to find the area of the square collider
        //Then, once this has been found it will define random spawn spoints with in the box
        //It should look something like..
        //A loop which iterates based on the amount of enemys needed to spawn
        //Then, generate a random spawn point in the area
        //and finally, instantiate the object at the given position. 
    }
}