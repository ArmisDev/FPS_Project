using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(AudioSource))]
public class Equipment_Flashlight : MonoBehaviour
{
    private PlayerInput input;
    private Light flashlight;
    private InputAction flashlightToggle;
    private AudioSource audioSource;
    [SerializeField] private AudioClip flashlightToggleSound;
    [SerializeField] private bool isFlashlightOn = false;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        flashlightToggle = input.actions["Flashlight"];
        flashlight = GetComponentInChildren<Light>();
        if(flashlight == null)
        {
            Debug.LogError("Please add to the child of flashlight component!!");
        }
        if (!isFlashlightOn) flashlight.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(flashlightToggle.WasPerformedThisFrame())
        {
            isFlashlightOn = !isFlashlightOn; // Toggle the flashlight state
            flashlight.gameObject.SetActive(isFlashlightOn);
            if (flashlightToggleSound != null)
            {
                audioSource.PlayOneShot(flashlightToggleSound);
            }
        }
    }
}
