using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCamera : MonoBehaviour
{
    [SerializeField] private bool isDefaultCam;
    [SerializeField] private ActiveCameraObj activeCamera;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (isDefaultCam)
            activeCamera.DefaultCamera = cam;
    }

    private void OnEnable()
    {
        activeCamera.SetNewCamera(cam);
    }

    private void OnDisable()
    {
        
    }
}
