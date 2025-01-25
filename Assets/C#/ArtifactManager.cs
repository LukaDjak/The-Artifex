using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Artifact
{
    public string name;  // Name of the artifact
    public Sprite icon;  // Icon of the artifact (for UI)
    public string description;  // Description of the artifact
    public GameObject artifactPrefab;
}

public class ArtifactManager : MonoBehaviour
{
    public List<Artifact> artifacts;

    //called when enemy dies
    public void TryDropArtifact(Vector2 spawnPosition)
    {
        int randomChance = Random.Range(0, 10000);
        if (randomChance == 0 && GameManager.gameData.total_kills > 500)
            GiveRandomArtifact(spawnPosition);
    }

    private void GiveRandomArtifact(Vector2 spawnPosition)
    {
        //select a random artifact (if the player hasn't already collected it)
        List<Artifact> uncollectedArtifacts = new();

        foreach (var artifact in artifacts)
        {
            if (!GameManager.Instance.collectedArtifacts.Contains(artifact))
                uncollectedArtifacts.Add(artifact);
        }

        if (uncollectedArtifacts.Count > 0)
        {
            int randomIndex = Random.Range(0, uncollectedArtifacts.Count);
            Artifact artifactToGive = uncollectedArtifacts[randomIndex];

            // Spawn the artifact visual in the world
            GameObject artifactVisual = Instantiate(artifactToGive.artifactPrefab, spawnPosition, Quaternion.identity);
            artifactVisual.GetComponent<ArtifactPickup>().Setup(artifactToGive);
        }
        else
            Debug.Log("All artifacts have already been collected.");
    }
}