using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private bool gameOver = false;
    [HideInInspector] public bool isUIActive = false;

    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private AudioClip[] clickClips;

    private void Awake() => instance = this;
    
    public bool IsGameOver() => gameOver;

    public void GameOver()
    {
        gameOver = true;
        Invoke(nameof(ShowGameOver), 2f);
    }

    private void ShowGameOver()
    {
        GameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnButtonClick() => AudioManager.instance.PlaySFX(clickClips[Random.Range(0, clickClips.Length)]);
}