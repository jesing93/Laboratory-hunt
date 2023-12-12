using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool isPaused;
    private bool isGameStarted;
    private bool isGameEnded;
    [SerializeField]
    private GameObject playerPref;
    private List<GameObject> mimics = new();

    public bool IsPaused { get => isPaused; }
    public bool GameStarted { get => isGameStarted; }
    public List<GameObject> Mimics { get => mimics; set => mimics = value; }

    private void Awake()
    {
        instance = this;
        Time.timeScale = 0.0f;
    }

    void Update()
    {
        //Check pause input
        if (isGameStarted && !isGameEnded &&
            (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)))
        {
            if(isPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        isPaused = true;
        MenuController.instance.Pause();
        CameraController.instance.TogglePause(isPaused);
        DOTween.Kill("CamRot", false);
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        isPaused = false;
        MenuController.instance.Unpause();
        CameraController.instance.TogglePause(isPaused);
        Time.timeScale = 1.0f;
    }

    public void StartGame()
    {
        isGameStarted = true;
        foreach (GameObject mimic in Mimics)
        {
            mimic.GetComponent<EnemyController>().Init();
        }
        PlayerController.instance.StartGame();
        Unpause();
    }

    public void EndGame(bool isVictory)
    {
        isGameEnded = true;
        MenuController.instance.EndGame(isVictory);
        CameraController.instance.TogglePause(isGameEnded);
        DOTween.Kill("CamRot", false);
        Time.timeScale = 0;
    }

    public void InitializePlayer(Vector3 initPos)
    {
        Instantiate(playerPref, initPos, Quaternion.identity);
    }

    public void KillEnemy (GameObject mimic)
    {
        mimics.Remove(mimic);
        if (mimics.Count == 0 )
        {
            EndGame(true);
        }
    }

    //CellTypeEnum for avoid multiple corridors together
    public enum CellType {Corridor, Room};
}
