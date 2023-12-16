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
        virtualCamera.Follow = PlayerController.instance.Head.transform;
    }

    private void Start()
    {
        Debug.Log("Dolly");
        PlayDolly();
    }

    private void PlayDolly()
    {
        cart.m_Speed = 0.8f;
        StartCoroutine(EndDolly());
    }

    private IEnumerator EndDolly()
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.StartGame();
        Destroy(this);
    }
}
