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

    //Singleton
    public static CameraHolder instance;

    private void Awake()
    {
        instance = this;
        mainCamera = GetComponentInChildren<CameraController>();
        SwitchToTPS();
    }

    private void Update()
    {
        //Follow the player;
        transform.position = currentTransform.position;
    }

    public void SwitchToFPS()
    {
        currentTransform = FPSAnchor;
        mainCamera.SwitchToFPS();
    }

    public void SwitchToTPS()
    {
        currentTransform = TPSAnchor;
        mainCamera.SwitchToTPS();
    }
}
