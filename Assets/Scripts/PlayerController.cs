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
    private bool isJumping;
    private bool isFiring;

    //Movement
    private float moveSpeed = 6.0f;
    private float jumpPower = 1.0f;
    private float maxJumpHeight = 1.0f;
    private float maxJumpTime = 0.5f;
    private float initialJumpVelocity;
    private float gravityValue = -9.81f;
    private float groundedGravity = -.05f;

    private float hInput;
    private float vInput;
    private bool jumpInput;
    private bool fireInput;

    private Vector3 direction;
    private Vector3 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;

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

        //Gravity setup
        float timeToApex = maxJumpTime / 2;
        gravityValue = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 / maxJumpHeight) / timeToApex;
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
        Fire();
        Move();
        HandleAnimation();
        HandleGravity();
        Jump();
    }

    private void FixedUpdate()
    {
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
        jumpInput = Input.GetButtonDown("Jump");
        fireInput = Input.GetButton("Fire1");
    }

    private void Move()
    {
        //Calculate movement direction
        direction = (orientation.forward * vInput + orientation.right * hInput).normalized;
        //Vector3 movement = direction * moveSpeed * Time.deltaTime;

        currentMovement.x = direction.x * moveSpeed * Time.deltaTime;
        currentMovement.z = direction.z * moveSpeed * Time.deltaTime;

        currentRunMovement.x = direction.x * moveSpeed * 1.5f * Time.deltaTime;
        currentRunMovement.z = direction.z * moveSpeed * 1.5f * Time.deltaTime;

        if (!isFiring)
        {
            controller.Move(currentRunMovement);
        }
        else
        {
            controller.Move(currentMovement);
        }

        //Rotate to camera direction
        transform.rotation = orientation.rotation;
    }

    private void Fire()
    {
        //TODO Handle fire
        if (fireInput && isGrounded && !isJumping)
        {
            isFiring = true;
        }
        else
        {
            isFiring = false;
        }
    }

    private void Jump()
    {
        if (!isJumping && jumpInput && isGrounded)
        {
            isJumping = true;
            currentMovement.y = currentRunMovement.y = initialJumpVelocity;
            
            //velocity.y += Mathf.Sqrt(jumpPower * -3.0f * gravityValue);
        }
        else if(isGrounded) 
        {
            isJumping = false;
        }
        //velocity.y += gravityValue * Time.deltaTime;
        //controller.Move(velocity * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if(isGrounded)
        {
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else
        {
            currentMovement.y += gravityValue * Time.deltaTime;
            currentRunMovement.y += gravityValue * Time.deltaTime;
        }
    }

    private void HandleAnimation()
    {
        //TODO: Handle animation function
    }

    #endregion
}
