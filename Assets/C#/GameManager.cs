using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public static GameData gameData = new();
    [HideInInspector] public static Settings settings = new();

    [Header("Artifact System")]
    public List<Artifact> collectedArtifacts = new();

    private string currentSceneName;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadScene("Main");

        gameData.total_games_played = PlayerPrefs.GetInt("games", 0);
        gameData.total_kills = PlayerPrefs.GetInt("kills", 0);
        gameData.number_of_artifacts = PlayerPrefs.GetInt("artifacts", 0);
        gameData.total_aura = PlayerPrefs.GetInt("aura", 0);

        settings.audioVolume = PlayerPrefs.GetFloat("volume", 1);
        settings.musicVolume = PlayerPrefs.GetFloat("music", 1);
        settings.muteAudio = PlayerPrefs.GetInt("mute", 0);
    }

    public void LoadScene(string loadSceneName, string unloadSceneName = null)
    {
        if (unloadSceneName != null || currentSceneName != null)
            SceneManager.UnloadSceneAsync(unloadSceneName ?? currentSceneName);
        SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
        currentSceneName = loadSceneName;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("games", gameData.total_games_played);
        PlayerPrefs.SetInt("kills", gameData.total_kills);
        PlayerPrefs.SetInt("artifacts", gameData.number_of_artifacts);        
        PlayerPrefs.SetInt("aura", gameData.total_aura);
        
        PlayerPrefs.SetFloat("volume", settings.audioVolume);
        PlayerPrefs.SetFloat("music", settings.musicVolume);
        PlayerPrefs.SetInt("mute", settings.muteAudio);
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