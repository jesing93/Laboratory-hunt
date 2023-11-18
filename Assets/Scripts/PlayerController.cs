using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    #region variables
    //Variables
    private Vector3 velocity;
    private bool isGrounded;

    //Movement
    private float moveSpeed = 4.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private float hInput;
    private float vInput;

    private Vector3 direction;

    //Components
    private Camera pCamera;
    private Rigidbody rb;
    private CharacterController controller;
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private GameObject head;


    //Singletone
    public static PlayerController instance;

    public Transform Orientation { get => orientation; set => orientation = value; }
    public GameObject Head { get => head; }

    #endregion

    #region methods

    private void Awake()
    {
        //Singletone
        instance = this;

        //Components
        rb = GetComponent<Rigidbody>();
        pCamera = Camera.main;
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        IsGrounded();
        PlayerInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void IsGrounded()
    {
        isGrounded = controller.isGrounded;
    }

    private void PlayerInputs()
    {
        //Get Imputs
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
    }

    private void Move()
    {
        //Calculate movement direction
        direction = (orientation.forward * vInput + orientation.right * hInput).normalized;

        controller.Move(direction * moveSpeed * Time.deltaTime);

        //Rotate to camera direction
        transform.rotation = orientation.rotation;
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        velocity.y += gravityValue * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    #endregion
}
