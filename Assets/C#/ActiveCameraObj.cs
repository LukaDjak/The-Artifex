using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveCameraObj", menuName = "Artifax/ActiveCamera")]
public class ActiveCameraObj : ScriptableObject
{
    public Camera DefaultCamera { get; set; }
    public Camera CurrentCamera { get; private set; }

    public void SetNewCamera(Camera camera) 
    {
        if (CurrentCamera != null) 
        {
            CurrentCamera.gameObject.SetActive(false);
        }

        CurrentCamera = camera;
    }

    private void CheckLastCamera() 
    {
        // Checkirat dali postoji kamera okja je aktivna
        // Ako Ne, aktiviraj Defaultnu Kameru
    }
}
