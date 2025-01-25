using UnityEngine;

[System.Serializable]
public class Artifact
{
    public string name;           // Unique identifier
    public string description;    // Artifact description (for UI)
    public Sprite icon;           // Icon for the artifact (for UI)
    public GameObject prefab;     // Artifact prefab (for spawning)
}