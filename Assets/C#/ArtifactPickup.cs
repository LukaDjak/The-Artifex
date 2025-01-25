using UnityEngine;
using DG.Tweening;

public class ArtifactPickup : MonoBehaviour
{
    private string artifactName;
    private bool isCollected = false;
    private bool readyToCollect = false;

    // Setup the artifact's name
    public void Setup(string name)
    {
        artifactName = name;
        transform.DOScale(0.3f, 1f).OnComplete(() => readyToCollect = true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player") && readyToCollect)
        {
            isCollected = true;

            // Add to collected artifacts
            if (!GameManager.Instance.collectedArtifacts.Contains(artifactName))
            {
                GameManager.Instance.collectedArtifacts.Add(artifactName);
                GameManager.gameData.number_of_artifacts++;
            }

            // Destroy the pickup object
            Destroy(gameObject);
        }
    }
}