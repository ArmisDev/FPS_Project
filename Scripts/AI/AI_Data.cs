using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AI_Data
{
    public string id;
    public Vector3 position;
    public bool isAlive;

    // Constructor for AI_Data
    public AI_Data()
    {
        // Assign a unique identifier to each instance of AI_Data
        this.id = System.Guid.NewGuid().ToString();
        this.isAlive = true; // Assuming default state is alive
        // Position will be set when creating an instance based on the actual AI position in game
    }
}