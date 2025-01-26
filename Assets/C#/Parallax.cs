using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    [SerializeField] private float parallaxFactorX = 0.5f; // The speed at which the background moves horizontally

    private Transform cameraTransform;
    private Vector3 previousCameraPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        // Calculate the movement difference between the current and previous frame
        Vector3 cameraMovement = cameraTransform.position - previousCameraPosition;

        // Apply parallax effect to the background based on camera movement and factor
        Vector3 newPosition = transform.position + new Vector3(cameraMovement.x * parallaxFactorX, 0, 0);
        transform.position = newPosition;

        // Update the previous camera position for the next frame
        previousCameraPosition = cameraTransform.position;
    }
}
