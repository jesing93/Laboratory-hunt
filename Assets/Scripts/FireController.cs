using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private Transform anchor;

    private void Start()
    {
        anchor = PlayerController.instance.FirePoint;
    }
    private void Update()
    {
        transform.position = anchor.position;
        transform.rotation = anchor.rotation;
    }
}
