using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            TogglePause(!isPaused);
    }

    public void TogglePause(bool pause)
    {
        isPaused = pause;
        pauseMenuUI.SetActive(pause);
        Time.timeScale = pause ? 0.0f : 1.0f;
    }

    public void Load(string sceneName)
    {
        //ensure the game is unpaused when switching scenes
        if (isPaused)
            TogglePause(false);
        GameManager.Instance.LoadScene(sceneName);
    }

    public void ToggleAudio(bool toggle)
    {
        Debug.Log($"Audio: {(toggle ? "On" : "Off")}");
    }
}