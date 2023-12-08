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

    public bool IsPaused { get => isPaused; }
    public bool GameStarted { get => isGameStarted; }

    private void Awake()
    {
        instance = this;
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
        Time.timeScale = 0;
        MenuController.instance.Pause();
        CameraController.instance.togglePause();
    }

    private void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        MenuController.instance.OnContinue();
        CameraController.instance.togglePause();
    }

    public void StartGame()
    {
        isGameStarted = true;
        PlayerController.instance.StartGame();
        CameraController.instance.togglePause();
    }

    public void EndGame(bool isVictory)
    {
        isGameEnded = true;
        Time.timeScale = 0;
        MenuController.instance.EndGame(isVictory);
        CameraController.instance.togglePause();
    }

    public void InitializePlayer(Vector3 initPos)
    {
        Instantiate(playerPref, initPos, Quaternion.identity);
    }

    //CellTypeEnum for avoid multiple corridors together
    public enum CellType {Corridor, Room};
}
