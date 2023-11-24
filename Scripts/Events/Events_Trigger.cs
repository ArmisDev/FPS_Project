using System;
using UnityEngine;

namespace Project.Events
{
    [RequireComponent(typeof(SphereCollider))]
    public class Events_Trigger : MonoBehaviour
    {
        [SerializeField] private bool shouldDelete;
        public event Action OnEnter;
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                OnEnter?.Invoke();
                Debug.Log("OnEnter has been invoked");
                if (shouldDelete)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}