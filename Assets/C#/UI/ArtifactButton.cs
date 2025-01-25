using UnityEngine;
using UnityEngine.UI;

public class ArtifactButton : MonoBehaviour
{
    public string artifactName;       // Name of the artifact this button represents
    public GameObject lockOverlay;    // Overlay for uncollected artifacts

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        //check if the artifact is collected
        bool isCollected = GameManager.Instance.collectedArtifacts.Contains(artifactName);

        button.interactable = isCollected;
        lockOverlay.SetActive(!isCollected);

        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        Artifact artifact = GameManager.Instance.GetArtifactByName(artifactName);
        if (artifact != null)
            FindObjectOfType<ArtifactUI>().ShowPopup(artifact);
    }
}
