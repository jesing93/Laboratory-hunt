using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartDolly : MonoBehaviour
{
    //Components
    private CinemachineDollyCart cart;
    private CinemachineVirtualCamera virtualCamera;

    //Singleton
    public static StartDolly instance;

    private void Awake()
    {
        instance = this;
        cart = GetComponentInChildren<CinemachineDollyCart>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        virtualCamera.LookAt = PlayerController.instance.Head.transform;
        Camera.main.GetComponent<CinemachineBrain>().m_IgnoreTimeScale = true;
    }

    public void PlayDolly()
    {
        Time.timeScale = 1.0f;
        Camera.main.GetComponent<CinemachineBrain>().m_IgnoreTimeScale = false;
        cart.m_Speed = 0.8f;
        StartCoroutine(EndDolly());
    }

    private IEnumerator EndDolly()
    {
        yield return new WaitForSecondsRealtime(4f);
        GameManager.instance.StartGame();
        virtualCamera.Priority = 0;
        yield return new WaitForSecondsRealtime(2f);
        Destroy(transform.gameObject);
    }
}
