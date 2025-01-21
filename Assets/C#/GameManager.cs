using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public static GameData gameData = new();

    private string currentSceneName;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadScene("Main");

        gameData.total_games_played = PlayerPrefs.GetInt("games", 0);
        gameData.total_kills = PlayerPrefs.GetInt("kills", 0);
        gameData.number_of_artifacts = PlayerPrefs.GetInt("artifacts", 0);
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
    }
}

public class GameData
{
    public int total_games_played;
    public int total_kills;
    public int number_of_artifacts;
}