using DigitalRuby.PyroParticles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region variables
    //Variables
    private bool isGrounded;
    private bool isJumping;
    private bool isFiring;
    private bool FPSMode;

    //Movement
    private float moveSpeed = 2.0f;
    private float jumpPower = 1500.0f;

    private float hInput;
    private float vInput;
    private bool jumpInput;
    private bool cameraModeInput;
    private bool fireInput;

    private Vector3 direction;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;

    //Components
    private Camera pCamera;
    private Rigidbody rb;
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private GameObject head;
    private Animator animator;
    [SerializeField]
    private GameObject playerMesh;
    [SerializeField]
    private Transform firePoint;
    private FireController fireAnimController;


    //Singletone
    public static PlayerController instance;

    public Transform Orientation { get => orientation; set => orientation = value; }
    public GameObject Head { get => head; }
    public Transform FirePoint { get => firePoint; }

    #endregion

    #region methods

    private void Awake()
    {
        //Singletone
        instance = this;

        //Components
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        pCamera = Camera.main;
        fireAnimController = GetComponentInChildren<FireController>();

        //Initialize
        FPSMode = false;
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
        Jump();
        SwitchCamera();
        HandleAnimation();
    }

    private void FixedUpdate()
    {
    }

    private void IsGrounded()
    {
        //Cast a ray down from player position
        Ray ray = new Ray(transform.position, Vector3.down);

        //If collides, can jump
        if (Physics.Raycast(ray, 1.1f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void PlayerInputs()
    {
        //Get Imputs
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetButtonDown("Jump");
        fireInput = Input.GetButton("Fire1");
        cameraModeInput = Input.GetKeyDown(KeyCode.F);
    }

    private void Move()
    {
        if(hInput != 0 || vInput != 0)
        {
            //Calculate movement direction
            direction = (orientation.forward * vInput + orientation.right * hInput).normalized;
            //Vector3 movement = direction * moveSpeed * Time.deltaTime;

            currentMovement.x = direction.x * moveSpeed;
            currentMovement.z = direction.z * moveSpeed;

            currentRunMovement.x = currentMovement.x * 1.5f;
            currentRunMovement.z = currentMovement.z * 1.5f;

            currentMovement.y = currentRunMovement.y = rb.velocity.y;

            if (!isFiring)
            {
                rb.velocity = currentRunMovement;
            }
            else
            {
                rb.velocity = currentMovement;
            }
        }
        else
        {
            currentMovement = Vector3.zero;
            currentMovement.y = rb.velocity.y; 
            rb.velocity = currentMovement;
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
            fireAnimController.Fire();
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
            rb.AddForce(0, jumpPower, 0);
        }
        else if(isJumping && !jumpInput && isGrounded) 
        {
            isJumping = false;
        }
        //velocity.y += gravityValue * Time.deltaTime;
        //controller.Move(velocity * Time.deltaTime);
    }

    private void HandleAnimation()
    {
        //TODO: Handle animation function
        animator.SetFloat("MoveX", vInput);
        animator.SetFloat("MoveY", hInput);
        if(vInput != 0 || hInput != 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void SwitchCamera()
    {
        if (cameraModeInput)
        {
            if (FPSMode)
            {
                SwitchToTPS();
            }
            else
            {
                SwitchToFPS();
            }
        }
    }

    public void SwitchToTPS()
    {
        FPSMode = false;
        //playerMesh.SetActive(true);
        CameraHolder.instance.SwitchToTPS();
        HudController.instance.SwitchToTPS();
    }

    public void SwitchToFPS()
    {
        FPSMode = true;
        //playerMesh.SetActive(false);
        CameraHolder.instance.SwitchToFPS();
        HudController.instance.SwitchToFPS();
    }

    #endregion
}
