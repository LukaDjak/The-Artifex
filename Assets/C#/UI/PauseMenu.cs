using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Sprite audioBtnSprite;
    [SerializeField] private Sprite noAudioBtnSprite;
    [SerializeField] private Image audioBtn;
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
        Camera.main.GetComponent<AudioListener>().enabled = !pause;
        LevelManager.instance.isUIActive = pause;
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
        audioBtn.sprite = isAudioMuted ? noAudioBtnSprite : audioBtnSprite;
        GameManager.settings.muteAudio = isAudioMuted ? 1 : 0;
        float muteVolume = isAudioMuted ? -80f : GameManager.settings.audioVolume; // -80 dB is effectively muted
        audioMixer.SetFloat("MasterVolume", muteVolume); // Adjust master volume
        SaveAndLoad.SaveSettings(GameManager.settings);
    }
}