using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    private void Update()
    {
        //Follow the player;
        transform.position = PlayerController.instance.Head.transform.position;
    }
}
