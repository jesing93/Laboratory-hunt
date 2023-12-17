using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    //Components
    private AudioSource buttonClick;
    [SerializeField]
    private GameObject[] panels;
    [SerializeField]
    private GameObject[] settingsItems;
    [SerializeField]
    private AudioMixer audioMixer;

    //Vars
    private GameObject activePanel;

    private void Awake()
    {
        buttonClick = GetComponent<AudioSource>();
        Time.timeScale = 1.0f;
        LoadPrefs();
    }

    private void Start()
    {
        ApplyPrefs();
    }

    public void OnStartGame ()
    {
        buttonClick.Play();
        StartCoroutine(StartGameCoroutine());
    }

    public void OnHowToPlay()
    {
        ChangeToPanel(0);
    }

    public void OnHighScores ()
    {
        ChangeToPanel(1);
    }

    public void OnSettings()
    {
        ChangeToPanel(2);
    }

    public void OnBack ()
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

    public void OnExit()
    {
        buttonClick.Play();
        StartCoroutine(ExitCoroutine());
    }

    public void OnSettingsChanged()
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

    private void LoadPrefs()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            //Update UI
            settingsItems[0].GetComponent<Slider>().value = PlayerPrefs.GetFloat("MasterVolume");
            settingsItems[1].GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");
            settingsItems[2].GetComponent<Slider>().value = PlayerPrefs.GetFloat("SFXVolume");
            settingsItems[3].GetComponent<Slider>().value = PlayerPrefs.GetFloat("UIVolume");
        }
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
    }

    private IEnumerator ExitCoroutine()
    {
        yield return new WaitForSeconds(1);
        Application.Quit();
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }
}
