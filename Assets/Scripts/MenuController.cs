using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    //Singletone
    public static MenuController instance;

    //Components
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject endgamePanel;
    [SerializeField]
    private GameObject victoryTitle;
    [SerializeField]
    private GameObject victoryText;
    [SerializeField]
    private GameObject loseTitle;
    [SerializeField]
    private GameObject loseText;
    private AudioSource buttonClick;
    [SerializeField]
    private GameObject[] panels;
    [SerializeField]
    private GameObject[] settingsItems;
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private GameObject loadingScreen;
    private bool isLoadingSettings;

    //Vars
    private GameObject activePanel;

    private void Awake()
    {
        instance = this;
        buttonClick = GetComponent<AudioSource>();
        LoadPrefs();
    }

    private void Start()
    {
        ApplyPrefs();
    }

    public void OnContinue()
    {
        buttonClick.Play();
        GameManager.instance.Unpause();
    }

    public void OnHowToPlay()
    {
        ChangeToPanel(0);
    }

    public void OnSettings()
    {
        ChangeToPanel(1);
    }

    public void OnBack()
    {
        ChangeToPanel();
    }

    /// <summary>
    /// Change to the new panel or close current panel if no ID given
    /// </summary>
    /// <param name="panelId">New panel ID</param>
    private void ChangeToPanel(int panelId = -1)
    {
        buttonClick.Play();
        if (activePanel != null)
        {
            activePanel.SetActive(false);
        }
        if (panelId == -1)
        {
            activePanel = null;
        }
        else
        {
            activePanel = panels[panelId];
            activePanel.SetActive(true);
        }
    }

    public void OnNewGame()
    {
        buttonClick.Play();
        StartCoroutine(NextScene(1));
    }

    public void OnMainMenu()
    {
        buttonClick.Play();
        StartCoroutine(NextScene(0));
    }

    private IEnumerator NextScene(int sceneId)
    {
        Debug.Log("Loading...");
        DOTween.KillAll();
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene(sceneId);
    }

    public void Pause ()
    {
        canvas.SetActive(true);
        pausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void Unpause ()
    {
        ChangeToPanel();
        pausePanel.SetActive(false);
        canvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EndGame(bool isVictory)
    {
        canvas.SetActive(true);
        endgamePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (isVictory)
        {
            victoryTitle.SetActive(true);
            victoryText.SetActive(true);
        }
        else
        {
            loseTitle.SetActive(true);
            loseText.SetActive(true);
        }
    }

    public void StartGame()
    {
        canvas.SetActive(false);
        loadingScreen.SetActive(false);
    }

    public void OnSettingsChanged()
    {
        if (!isLoadingSettings)
        {
            //Save settings
            PlayerPrefs.SetFloat("MasterVolume", settingsItems[0].GetComponent<Slider>().value);
            PlayerPrefs.SetFloat("MusicVolume", settingsItems[1].GetComponent<Slider>().value);
            PlayerPrefs.SetFloat("SFXVolume", settingsItems[2].GetComponent<Slider>().value);
            PlayerPrefs.SetFloat("UIVolume", settingsItems[3].GetComponent<Slider>().value);
            PlayerPrefs.SetFloat("Sensibility", settingsItems[4].GetComponent<Slider>().value);
            //Applying to the audio mixer
            ApplyPrefs();
        }
    }

    private void LoadPrefs()
    {
        isLoadingSettings = true;
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            //Update UI
            settingsItems[0].GetComponent<Slider>().value = PlayerPrefs.GetFloat("MasterVolume");
            settingsItems[1].GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");
            settingsItems[2].GetComponent<Slider>().value = PlayerPrefs.GetFloat("SFXVolume");
            settingsItems[3].GetComponent<Slider>().value = PlayerPrefs.GetFloat("UIVolume");
            settingsItems[4].GetComponent<Slider>().value = PlayerPrefs.GetFloat("Sensibility");
        }
        isLoadingSettings = false;
    }

    private void ApplyPrefs()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            //Update audio mixer
            audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
            audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
            audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));
            audioMixer.SetFloat("UIVolume", PlayerPrefs.GetFloat("UIVolume"));
        }
        if(PlayerController.instance != null)
        {
            PlayerController.instance.UpdateSensibility();
        }
    }
}
