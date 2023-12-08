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

    private void Awake()
    {
        instance = this;
    }

    public void OnContinue()
    {
        Unpause();
    }

    public void OnNewGame()
    {
        StartCoroutine(LoadScene(1));
    }

    public void OnMainMenu()
    {
        StartCoroutine(LoadScene(0));
    }

    private IEnumerator LoadScene(int sceneId)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneId);
    }

    public void Pause ()
    {
        this.gameObject.SetActive(true);
        pausePanel.SetActive(true);
    }

    public void Unpause ()
    {
        pausePanel.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void EndGame(bool isVictory)
    {
        this.gameObject.SetActive(true);
        endgamePanel.SetActive(true);
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
