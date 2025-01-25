using DG.Tweening;
using UnityEngine;

public class ArtifactPickup : MonoBehaviour
{
    private Artifact artifact;  // The artifact this visual represents
    private bool isCollected = false;

    // Called by ArtifactManager to set the artifact data
    public void Setup(Artifact artifact)
    {
        this.artifact = artifact;
        transform.DOScale(.3f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player is in range to collect the artifact
        if (!isCollected && other.CompareTag("Player"))
        {
            isCollected = true;
            GameManager.Instance.collectedArtifacts.Add(artifact);
            GameManager.gameData.number_of_artifacts++;

            // Destroy the visual representation of the artifact
            Destroy(gameObject);

            Debug.Log($"Artifact {artifact.name} collected by player!");
        }
    }
}
