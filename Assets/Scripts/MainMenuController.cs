using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    //Components
    private AudioSource buttonClick;

    private void Awake()
    {
        buttonClick = GetComponent<AudioSource>();
    }
    public void OnStartGame ()
    {
        buttonClick.Play();
        StartCoroutine(StartGameCoroutine());
    }

    public void OnHighScores ()
    {
        buttonClick.Play();
    }

    public void OnBack ()
    {
        buttonClick.Play();
    }

    public void OnExit()
    {
        buttonClick.Play();
        StartCoroutine(ExitCoroutine());
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
