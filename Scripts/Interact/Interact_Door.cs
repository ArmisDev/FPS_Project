using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Interaction
{
    [RequireComponent(typeof(Animator), typeof(InteractableObject))]
    public class Interact_Door : MonoBehaviour
    {
        private InteractableObject interactableObject;
        private Animator animator;
        private GameObject player;

        [SerializeField] private float rotationSpeed = 90f; // Adjust this to control how fast the door opens/closes
        private Vector3 initRot;
        private bool isOpen;

        private void Awake()
        {
            interactableObject = GetComponent<InteractableObject>();
            interactableObject.OnInteractWithObject += HandleDoor;
            player = GameObject.FindGameObjectWithTag("Player");
            animator = GetComponent<Animator>();
            initRot = transform.localEulerAngles;
        }

        private void OnDestroy()
        {
            interactableObject.OnInteractWithObject -= HandleDoor;
        }

        void HandleDoor()
        {
            if (!isOpen)
            {
                StartCoroutine(OpenDoor());
            }
            else
            {
                StartCoroutine(CloseDoor());
            }
        }

        private IEnumerator OpenDoor()
        {
            isOpen = true;
            float targetAngle = 90f; // Angle to open the door to

            while (transform.localRotation.eulerAngles.y < targetAngle)
            {
                float step = rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up, step);
                yield return null;
            }
        }

        private IEnumerator CloseDoor()
        {
            isOpen = false;

            float targetAngle = initRot.y;

            while (Mathf.Abs(transform.localEulerAngles.y - targetAngle) > 0.01f)
            {
                float step = rotationSpeed * Time.deltaTime;
                float currentAngle = Mathf.MoveTowardsAngle(transform.localEulerAngles.y, targetAngle, step);
                transform.localEulerAngles = new Vector3(initRot.x, currentAngle, initRot.z);
                yield return null;
            }
        }
    }
}
