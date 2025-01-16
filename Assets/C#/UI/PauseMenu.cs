using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    private bool isPaused = false;
    private bool isAudioMuted = false;
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

    public void ToggleAudio()
    {
        isAudioMuted = !isAudioMuted;
        Debug.Log($"Audio: {(isAudioMuted ? "On" : "Off")}");
    }
}