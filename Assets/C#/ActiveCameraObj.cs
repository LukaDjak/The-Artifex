using UnityEngine;

[CreateAssetMenu(fileName = "ActiveCameraObj", menuName = "TheArtifex/ActiveCamera")]
public class ActiveCameraObj : ScriptableObject
{
    public Camera DefaultCamera { get; set; }
    public Camera CurrentCamera { get; private set; }

    public void SetNewCamera(Camera camera)
    {
        if (CurrentCamera != null)
            CurrentCamera.gameObject.SetActive(false);

        CurrentCamera = camera;
    }

    public void CheckLastCamera()
    {
        //check if there's a camera that's active
        //if there's not, activate the default one - LOADING SCREEN in the future
        if (CurrentCamera == null || !CurrentCamera.gameObject.activeInHierarchy)
        {
            DefaultCamera.gameObject.SetActive(true);
            CurrentCamera = DefaultCamera;
        }
    }
}