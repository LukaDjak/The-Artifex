using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadScene("Main");
    }

    public void LoadScene(string loadSceneName, string unloadSceneName = null)
    {
        if (unloadSceneName != null)
            SceneManager.UnloadSceneAsync(unloadSceneName);
        SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
    }
}
