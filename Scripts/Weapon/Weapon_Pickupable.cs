using UnityEngine;

public class Weapon_Pickupable : MonoBehaviour
{
    public Weapon_SCR scriptableObject;

    public void DestroyObject()
    {
        Destroy(gameObject, 0.1f);
    }
}
