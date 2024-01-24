using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Health : MonoBehaviour
{
    [SerializeField] private Player_Health playerHealth;
    [SerializeField] private TextMeshProUGUI healthText;

    // Start is called before the first frame update
    void Awake()
    {
        if(playerHealth == null || healthText == null)
        {
            Debug.LogWarning("Please fill in components!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = playerHealth.health.ToString("F0");
    }
}