using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Requirement
{
    public enum RequirementType
    {
        TotalGamesPlayed,
        TotalKills,
        NumberOfArtifacts
    }

    public RequirementType type;           //type of the requirement (unfortunately, its hardcoded) 
    public string requirementName;         //description of the requirement
    public int requiredAmount;             //the required amount for the task

    public TMP_Text description;           //text field for the task description
    public TMP_Text progressText;          //text field for showing progress (e.g., "5/10")
    public Slider progressionSlider;       //slider to visually represent the progress

    public void UpdateProgressUI()
    {
        int currentAmount = GetCurrentAmountFromGameManager();

        description.text = requirementName;
        progressText.text = $"{currentAmount}/{requiredAmount}";

        progressionSlider.maxValue = requiredAmount;
        progressionSlider.value = currentAmount;
    }

    public bool IsCompleted() => GetCurrentAmountFromGameManager() >= requiredAmount;

    private int GetCurrentAmountFromGameManager()
    {
        //fetch the appropriate data from GameManager based on the type
        return type switch
        {
            RequirementType.TotalGamesPlayed => GameManager.gameData.total_games_played,
            RequirementType.TotalKills => GameManager.gameData.total_kills,
            RequirementType.NumberOfArtifacts => GameManager.gameData.number_of_artifacts,
            _ => 0,
        };
    }
}

public class MapUnlocker : MonoBehaviour
{
    public string mapName;

    public GameObject lockedButton;
    public GameObject selectButton;
    public GameObject requirementsUI;

    public Requirement[] requirements;

    private void Start()
    {
        ToggleButtons();
        UpdateUI();
    }

    public void ToggleRequirementsUI() => requirementsUI.SetActive(!requirementsUI.activeInHierarchy);

    private void ToggleButtons()
    {
        lockedButton.SetActive(!IsUnlocked());
        selectButton.SetActive(IsUnlocked());
    }

    public bool IsUnlocked()
    {
        //check if all requirements are fulfilled
        foreach (Requirement requirement in requirements)
        {
            if (!requirement.IsCompleted())
                return false;
        }
        return true;
    }

    public void UpdateUI()
    {
        foreach (Requirement requirement in requirements)
            requirement.UpdateProgressUI();
    }
}