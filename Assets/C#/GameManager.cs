using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public static GameData gameData = new();
    [HideInInspector] public static Settings settings = new();
    [HideInInspector] public List<string> collectedArtifacts;

    [Header("Artifact System")]
    public List<Artifact> allArtifacts;

    private string currentSceneName;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadScene("Main");

        PlayerPrefs.DeleteAll();

        gameData = SaveAndLoad.LoadGameData();
        settings = SaveAndLoad.LoadSettings();
        collectedArtifacts = SaveAndLoad.LoadCollectedArtifacts();
    }

    public void LoadScene(string loadSceneName, string unloadSceneName = null)
    {
        if (unloadSceneName != null || currentSceneName != null)
            SceneManager.UnloadSceneAsync(unloadSceneName ?? currentSceneName);
        SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
        currentSceneName = loadSceneName;
    }
    public Artifact GetArtifactByName(string artifactName) => allArtifacts.Find(artifact => artifact.name == artifactName);

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
}

public class Settings
{
    public float audioVolume;
    public float musicVolume;
    [Range(0,1)] public int muteAudio = 0; //0-1
}