using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_SaveData : MonoBehaviour, IDataPersistance
{
    private Scene currentScene;

    public void SaveData(ref GameData data)
    {
        currentScene = SceneManager.GetActiveScene();
        if (!string.IsNullOrEmpty(currentScene.name))
        {
            data.CurrentSceneName = currentScene.name;
        }
        else
        {
            Debug.LogError("Scene_SaveData: Current scene name is invalid.");
        }
    }

    public void LoadData(GameData data)
    {
        if (!string.IsNullOrEmpty(data.CurrentSceneName))
        {
            SceneManager.LoadScene(data.CurrentSceneName);
        }
        else
        {
            Debug.LogError("Scene_SaveData: Attempted to load invalid scene name.");
        }
    }
}
