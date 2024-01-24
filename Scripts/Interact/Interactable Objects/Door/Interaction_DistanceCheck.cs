using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_DistanceCheck : MonoBehaviour
{
    [HideInInspector] public float dotProduct;
    private GameObject player;
    public bool playerInFrontOrBehind = false; // This variable will determine if the player is in front or behind the door

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerStay(Collider other)
    {
        if (player != null)
        {
            PlayerDotProduct();
        }
    }

    void PlayerDotProduct()
    {
        Vector3 playerDirection = player.transform.position;
        Vector3 doorDirection = (transform.position - player.transform.position).normalized;
        dotProduct = Vector3.Dot(playerDirection, doorDirection);

        // Calculate the angle between the player's forward vector and the door's forward vector
        float angle = Vector3.Angle(playerDirection, doorDirection);

        // Check if the angle is close to 180 degrees (player in front or behind)
        playerInFrontOrBehind = Mathf.Approximately(angle, 180.0f);
        Debug.Log("Dot Product: " + dotProduct);
        Debug.Log("Player in Front or Behind: " + playerInFrontOrBehind);
    }
}