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
    private bool isDead;
    private bool isOverheat;
    private bool isGameStarted;
    private bool FPSMode;
    private LayerMask groundLayer;

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

    //Stats
    private float currentHealth = 100;
    private float maxHealth = 100;
    private float healthRegenTime;
    private float healthRegenDelay = 10f;
    private float currentHeat;
    private float maxHeat = 50;
    private float heatRegenTime;
    private float heatRegenDelay = 0.5f;
    private float overheatDelay;
    private float overheatTime = 2f;

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

    //Get-Set
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

        groundLayer = LayerMask.GetMask("Cells");

        //Initialize
        FPSMode = false;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //isGameStarted = true; //TODO: Delete once gameflow is finished
    }

    private void Update()
    {
        if (!isDead && isGameStarted && Time.timeScale > 0)
        {
            IsGrounded();
            PlayerInputs();
            Fire();
            HandleHeat();
            Move();
            HandleHealthRegen();
            //Jump();
            SwitchCamera();
            HandleAnimation();
        }
    }

    private void FixedUpdate()
    {
    }

    /// <summary>
    /// Ground check
    /// </summary>
    private void IsGrounded()
    {
        //Cast a ray down from player position
        Ray ray = new Ray(transform.position, Vector3.down);

        //If collides, is grounded
        if (Physics.Raycast(ray, 1.1f, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    /// <summary>
    /// Receive player inputs
    /// </summary>
    private void PlayerInputs()
    {
        //Get Imputs
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
        //jumpInput = Input.GetButtonDown("Jump");
        fireInput = Input.GetButton("Fire1");
        cameraModeInput = Input.GetKeyDown(KeyCode.F);
    }

    /// <summary>
    /// Handle player movement
    /// </summary>
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

    /// <summary>
    /// Handle flamethrower firing
    /// </summary>
    private void Fire()
    {
        //TODO Handle fire
        if (fireInput && !isOverheat)
        {
            isFiring = true;
            fireAnimController.Fire();
        }
        else
        {
            isFiring = false;
        }
    }

    /// <summary>
    /// Handle jump. Discarded.
    /// </summary>
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

    /// <summary>
    /// Handle player animation
    /// </summary>
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

    /// <summary>
    /// Switch between FPS and TPS
    /// </summary>
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

    /// <summary>
    /// Switch to the TPS camera
    /// </summary>
    public void SwitchToTPS()
    {
        FPSMode = false;
        //playerMesh.SetActive(true);
        CameraHolder.instance.SwitchToTPS();
        HudController.instance.SwitchToTPS();
    }


    /// <summary>
    /// Switch to the FPS camera
    /// </summary>
    public void SwitchToFPS()
    {
        FPSMode = true;
        //playerMesh.SetActive(false);
        CameraHolder.instance.SwitchToFPS();
        HudController.instance.SwitchToFPS();
    }

    /// <summary>
    /// Public function to receive attacks damage
    /// </summary>
    /// <param name="damage"></param>
    public void ReceiveDamage(float damage)
    {
        currentHealth = MathF.Max(currentHealth - damage, 0);
        HudController.instance.UpdateHealth(currentHealth);
        healthRegenTime = Time.time;
        if (currentHealth == 0)
        {
            //If 0 health, die
            isDead = true;
            GameManager.instance.EndGame(false);
        }
    }

    /// <summary>
    /// Handle the flamethrower heat, overheat and cooling
    /// </summary>
    private void HandleHeat()
    {
        //Overheat delay
        if (isOverheat)
        {
            if (overheatTime + overheatDelay < Time.time)
            {
                //Cancel overheat after a delay
                isOverheat = false;
                HudController.instance.Overheat(false);
            }
        }
        else
        {
            if (isFiring)
            {
                currentHeat = Mathf.Min(currentHeat + Time.deltaTime * 8, maxHeat);
                HudController.instance.UpdateHeat(currentHeat);
                    heatRegenTime = Time.time;
                //Overheat if heat is maxed
                if (currentHeat == maxHeat)
                {
                    isOverheat = true;
                    overheatDelay = Time.time;
                    HudController.instance.Overheat(true);
                }
            }
            else if (currentHeat > 0 && heatRegenTime + heatRegenDelay < Time.time)
            {
                //Start reducing heat a few seconds after firing
                currentHeat = Mathf.Max(currentHeat - Time.deltaTime * 8, 0);
                HudController.instance.UpdateHeat(currentHeat);
            }
        }
    }

    /// <summary>
    /// Handle the player health regen
    /// </summary>
    private void HandleHealthRegen()
    {
        if(currentHealth < maxHealth && healthRegenTime + healthRegenDelay < Time.time)
        {
            currentHealth = Mathf.Min(currentHealth + Time.deltaTime * 2, maxHealth);
            HudController.instance.UpdateHealth(currentHealth);
        }
    }

    public void StartGame()
    {
        isGameStarted = true;
    }

    #endregion
}
