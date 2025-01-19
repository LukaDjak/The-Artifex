using System;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private bool gameOver = false;

    [SerializeField] private GameObject GameOverUI;

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
}