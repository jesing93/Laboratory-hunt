using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    //Components
    [SerializeField]
    private GameObject fpsHud;
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Slider heatSlider;
    [SerializeField]
    private TMP_Text overheatText;

    //Vars
    private Gradient overheatGrad = new();
    private Coroutine overheatBlink;

    //Singletone
    public static HudController instance;

    private void Awake()
    {
        instance = this;
        SwitchToTPS();

        //Gradients
        GradientColorKey[] colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(new Color32(209, 53, 3, 255), 1.0f);
        colors[1] = new GradientColorKey(new Color32(230, 25, 1, 255), 1.0f);
        overheatGrad.SetKeys(colors, new GradientAlphaKey[2]);
    }

    /// <summary>
    /// Change to FPS Hud
    /// </summary>
    public void SwitchToFPS()
    {
        //FPSMode = true;
        fpsHud.SetActive(true);
    }

    /// <summary>
    /// Change to TPS Hud
    /// </summary>
    public void SwitchToTPS()
    {
        //FPSMode = false;
        fpsHud.SetActive(false);
    }

    /// <summary>
    /// Update health slider value
    /// </summary>
    /// <param name="currentHealth"></param>
    public void UpdateHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    /// <summary>
    /// Update heat slider value
    /// </summary>
    /// <param name="currentHeat"></param>
    public void UpdateHeat(float currentHeat)
    {
        heatSlider.value = currentHeat;
    }
    
    /// <summary>
    /// Update heat slider color onn overheat
    /// </summary>
    /// <param name="isOverheat">If the flamethrower is in overheat or not</param>
    public void Overheat (bool isOverheat)
    {
        if (isOverheat)
        {
            transform.Find("Canvas/StatsPanel/HeatSlider/FillArea/Fill")
                .GetComponent<Image>().color = new Color32(230, 25, 1, 255);
            overheatBlink = StartCoroutine(overheatTextBlink());
        }
        else
        {
            transform.Find("Canvas/StatsPanel/HeatSlider/FillArea/Fill")
                .GetComponent<Image>().color = new Color32(209, 53, 3, 255);
            StopCoroutine(overheatBlink);
            overheatText.gameObject.SetActive(false);
        }
    }

    private IEnumerator overheatTextBlink()
    {
        overheatText.gameObject.SetActive(true);
        while (true)
        {
            overheatText.faceColor = Color.white;
            yield return new WaitForSeconds(0.5f);
            overheatText.faceColor = Color.black;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
