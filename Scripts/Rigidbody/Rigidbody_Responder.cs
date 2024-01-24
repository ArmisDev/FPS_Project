using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rigidbody_Responder : MonoBehaviour
{
    public void HitResponse(RaycastHit hit, float damage)
    {
        hit.rigidbody.AddForce(-hit.normal * damage, ForceMode.Impulse);
    }
}
