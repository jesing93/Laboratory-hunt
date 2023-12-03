using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    [SerializeField]
    private Transform FPSAnchor;
    [SerializeField]
    private Transform TPSAnchor;
    private Transform currentTransform;
    private CameraController mainCamera;

    private bool FPSMode;
    private Vector3 FPSOffset;

    //Singleton
    public static CameraHolder instance;

    private void Awake()
    {
        instance = this;
        mainCamera = GetComponentInChildren<CameraController>();
        FPSOffset = new Vector3(0f, 0f, 0f);
        SwitchToTPS();
    }

    private void Update()
    {
        //Follow the player;
        if (FPSMode)
        {
            transform.position = currentTransform.position + FPSOffset;
        }
        else
        {
            transform.position = currentTransform.position;
        }
        
    }

    public void SwitchToFPS()
    {
        FPSMode = true;
        currentTransform = FPSAnchor;
        mainCamera.SwitchToFPS();
    }

    public void SwitchToTPS()
    {
        FPSMode = false;
        currentTransform = TPSAnchor;
        mainCamera.SwitchToTPS();
    }
}
