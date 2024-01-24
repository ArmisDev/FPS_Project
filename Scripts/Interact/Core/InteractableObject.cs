using System;
using UnityEngine;

namespace Project.Interaction
{
    public class InteractableObject : MonoBehaviour
    {
        [HideInInspector] public GameObject interactionSender;
        public event Action OnInteractWithObject;
        public RaycastHit hit;

        public void CallInteractionEvent(GameObject sender, RaycastHit hit = new RaycastHit())
        {
            interactionSender = sender;
            OnInteractWithObject?.Invoke();
            hit = this.hit;
        }
    }
}