using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private string currentSceneName;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadScene("Main");
    }

    public void LoadScene(string loadSceneName, string unloadSceneName = null)
    {
        if (unloadSceneName != null || currentSceneName != null)
            SceneManager.UnloadSceneAsync(unloadSceneName != null ? unloadSceneName : currentSceneName);
        SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
        currentSceneName = loadSceneName;
    }
}
