using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    //private bool FPSMode;
    [SerializeField]
    private GameObject fpsHud;

    //Singletone
    public static HudController instance;

    private void Awake()
    {
        instance = this;
        SwitchToTPS();
    }

    public void SwitchToFPS()
    {
        //FPSMode = true;
        fpsHud.SetActive(true);
    }
    public void SwitchToTPS()
    {
        //FPSMode = false;
        fpsHud.SetActive(false);
    }
}
