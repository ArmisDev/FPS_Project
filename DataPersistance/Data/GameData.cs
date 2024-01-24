using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameData
{
    public float playerHealth;
    public Vector3 playerPos;

    //Weapons
    public GameObject CurrentWeapon;
    public string CurrentWeaponID;
    public List<string> InventoryWeaponIDs;
    public List<string> DroppedWeaponIDs;
    public List<GameObject> CurrentWeapons;
    public List<GameObject> DroppedWeapons;

    //Scene
    public Scene CurrentScene;
    public string CurrentSceneName;

    public GameData()
    {
        this.playerHealth = 100;
        playerPos = Vector3.zero;
    }
}