using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Mover : MonoBehaviour
{
    private Vector3 previousPosition;
    public float smoothedSpeed;
    public float smoothingFactor = 0.1f; // Adjust this value to control the smoothing

    void Start()
    {
        previousPosition = transform.position;
    }

    float CalculateSpeed()
    {
        // Calculate the instantaneous speed
        float distance = Vector3.Distance(transform.position, previousPosition);
        float instantaneousSpeed = distance / Time.deltaTime;

        // Apply smoothing to the speed value
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, instantaneousSpeed, smoothingFactor * Time.deltaTime);

        // Update previousPosition for the next frame
        previousPosition = transform.position;

        // Optionally, output the smoothed speed to the console
        return smoothedSpeed;
    }

    private void Update()
    {
        CalculateSpeed();
    }
}