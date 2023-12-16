using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        instance = this;
        buttonClick = GetComponent<AudioSource>();
    }

    public void OnContinue()
    {
        buttonClick.Play();
        GameManager.instance.Unpause();
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
}
