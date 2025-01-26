using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public static GameData gameData = new();
    [HideInInspector] public static Settings settings = new();
    [HideInInspector] public List<string> collectedArtifacts;
    public Camera loaderCamera; // Assign this in the inspector (camera in the loader scene)

    public List<Artifact> artifacts;

    private string currentSceneName;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadScene("Main");

        gameData = SaveAndLoad.LoadGameData();
        settings = SaveAndLoad.LoadSettings();
        collectedArtifacts = SaveAndLoad.LoadCollectedArtifacts();
    }

    public void LoadScene(string loadSceneName, string unloadSceneName = null)
    {
        if (!string.IsNullOrEmpty(unloadSceneName) || !string.IsNullOrEmpty(currentSceneName))
        {
            loaderCamera.gameObject.SetActive(true);
            SceneManager.UnloadSceneAsync(unloadSceneName ?? currentSceneName);
        }

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);

        loadOperation.completed += (operation) =>
        {
            if (loaderCamera != null)
                loaderCamera.gameObject.SetActive(false);
        };
        currentSceneName = loadSceneName;
    }

    public Artifact GetArtifactByName(string artifactName) => artifacts.Find(artifact => artifact.name == artifactName);

    private void OnApplicationQuit()
    {
        SaveAndLoad.SaveGameData(gameData);
        SaveAndLoad.SaveSettings(settings);
        SaveAndLoad.SaveCollectedArtifacts(collectedArtifacts);
    }
}

public class GameData
{
    public int total_games_played;
    public int total_kills;
    public int number_of_artifacts;
    public int total_aura;
    public int chance_to_respawn;
}

public class Settings
{
    public float audioVolume;
    public float musicVolume;
    [Range(0,1)] public int muteAudio = 0; //0-1
}