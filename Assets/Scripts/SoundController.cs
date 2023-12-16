using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource loadingAudio;
    [SerializeField] private AudioSource alarmAudio;
    [SerializeField] private AudioSource levelAudio;
    [SerializeField] private AudioSource winAudio;
    [SerializeField] private AudioSource loseAudio;

    //Singleton
    public static SoundController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartLevel()
    {
        loadingAudio.DOFade(0, 1);
        alarmAudio.Play();
        alarmAudio.DOFade(0, 8.1f);
        levelAudio.volume = 0;
        levelAudio.Play();
        levelAudio.DOFade(50, 4);
    }

    public void EndLevel(bool win)
    {
        levelAudio.Stop();
        if (win)
        {
            //winAudio.Play();
        }
        else
        {
            //loseAudio.Play();
        }
    }
}
