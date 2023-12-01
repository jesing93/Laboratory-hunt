using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public class Cell : MonoBehaviour
{
    [HideInInspector]
    public BoxCollider TriggerBox;
    public GameObject[] Exits;
    public GameManager.CellType cellType;

    private void Awake()
    {
        TriggerBox = GetComponent<BoxCollider>();
        TriggerBox.isTrigger = true;
    }
}
