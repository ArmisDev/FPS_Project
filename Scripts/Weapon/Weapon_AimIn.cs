using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Weapon_AimIn : MonoBehaviour
{
    [SerializeField] private Vector3 desiredPos;
    [SerializeField] private Vector3 desiredRotation;
    [SerializeField] private float aimInAccel;
    private Transform aimInTransform;
    private Vector3 initPos;
    private Vector3 initRotation;
    public float aimInIncrement;
    private bool isAiming;
    private PlayerInput input;
    private InputAction aimIn;

    // Start is called before the first frame update
    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        aimIn = input.actions["AimIn"];
    }

    // Update is called once per frame
    void Update()
    {
        float aimInputValue = aimIn.ReadValue<float>();
        bool isAimingInput = aimInputValue > 0;

        if (isAimingInput)
        {
            if (!isAiming)
            {
                // Reset increment when starting to aim in
                aimInIncrement = 0;
            }

            aimInIncrement += Time.deltaTime * aimInAccel;
            aimInIncrement = Mathf.Clamp(aimInIncrement, 0, 1); // Clamp to ensure it stays within range

            UpdateAimTransform(true);
            isAiming = true;
        }
        else if (!isAimingInput && isAiming)
        {
            // Aim out logic
            aimInIncrement -= Time.deltaTime * aimInAccel;
            aimInIncrement = Mathf.Clamp(aimInIncrement, 0, 1); // Clamp to ensure it stays within range

            UpdateAimTransform(false);

            // Check if the initial position is reached
            if (aimInIncrement == 0)
            {
                isAiming = false;
            }
        }
        else if (aimInputValue == 0 && !isAiming)
        {
            // No aiming input and not currently aiming
            aimInIncrement = 0;
        }
    }

    void UpdateAimTransform(bool isAimingIn)
    {
        Vector3 targetPosition = isAimingIn ? desiredPos : initPos;
        Vector3 targetRotation = isAimingIn ? desiredRotation : initRotation;

        aimInTransform.localPosition = Vector3.Lerp(
            aimInTransform.localPosition, targetPosition, aimInIncrement);

        aimInTransform.localEulerAngles = Vector3.Lerp(
            aimInTransform.localEulerAngles, targetRotation, aimInIncrement);
    }
}
