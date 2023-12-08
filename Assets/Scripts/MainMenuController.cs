using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnStartGame ()
    {
        StartCoroutine(StartGameCoroutine());
    }

    public void OnHighScores ()
    {

    }

    public void OnBack ()
    {

    }

    public void OnExit()
    {
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
