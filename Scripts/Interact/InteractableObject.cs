using System;
using UnityEngine;

namespace Project.Interaction
{
    public class InteractableObject : MonoBehaviour
    {
        public event Action OnInteractWithObject;

        public void CallInteractionEvent()
        {
            OnInteractWithObject?.Invoke();
        }
    }
}