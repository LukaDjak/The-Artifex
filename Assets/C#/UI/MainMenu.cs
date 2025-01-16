using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels = new List<GameObject>();

    public void LoadLevel(string sceneName) => GameManager.Instance.LoadScene(sceneName, "Main");

    public void QuitGame() => Application.Quit();

    public void TogglePanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
            panels[i].SetActive(i == index);
    }
}