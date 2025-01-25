using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactUI : MonoBehaviour
{
    [SerializeField] private GameObject popupUI;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;

    public void ShowPopup(Artifact artifact)
    {
        popupUI.SetActive(true);
        nameText.text = artifact.name;
        descriptionText.text = artifact.description;
        iconImage.sprite = artifact.icon;
    }

    public void ClosePopup()
    {
        popupUI.SetActive(false);
    }
}
