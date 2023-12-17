using DigitalRuby.PyroParticles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using DG.Tweening;
using System.Threading;

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
    private float rotSensibility = 400;
    private float rotDelay = 0.25f;
    private float rotation;

    private LayerMask groundLayer;

    //Movement
    private float moveSpeed = 2.0f;
    private float jumpPower = 1500.0f;

    private float hInput;
    private float vInput;
    private bool jumpInput;
    private bool cameraModeInput;
    private bool fireInput;
    private float hCamInput;
    private float vCamInput;

    private Vector3 direction;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;

    //Stats
    private float currentHealth = 100;
    private readonly float maxHealth = 100;
    private float healthRegenTime;
    private readonly float healthRegenDelay = 10f;
    private float currentHeat;
    private readonly float maxHeat = 50;
    private float heatRegenTime;
    private readonly float heatRegenDelay = 0.5f;
    private float overheatDelay;
    private readonly float overheatTime = 2f;

    //Components
    private Rigidbody rb;
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform lookAtTarget;
    [SerializeField]
    private GameObject head;
    private Animator animator;
    [SerializeField]
    private GameObject playerMesh;
    [SerializeField]
    private Transform firePoint;
    private FireController fireAnimController;
    private CinemachineVirtualCamera FPSCamera;
    private CinemachineVirtualCamera TPSCamera;

    //Singletone
    public static PlayerController instance;

    //Get-Set
    public Transform Orientation { get => orientation; set => orientation = value; }
    public GameObject Head { get => head; }
    public Transform FirePoint { get => firePoint; }
    public Rigidbody Rb { get => rb; }

    #endregion

    #region methods

    private void Awake()
    {
        //Singletone
        instance = this;

        //Components
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        fireAnimController = GetComponentInChildren<FireController>();
        //Init FPS camera
        FPSCamera = GameObject.FindGameObjectWithTag("FPVC").GetComponent<CinemachineVirtualCamera>();
        FPSCamera.Follow = head.transform;
        FPSCamera.LookAt = lookAtTarget;
        //Init TPS camera
        TPSCamera = GameObject.FindGameObjectWithTag("TPVC").GetComponent<CinemachineVirtualCamera>();
        TPSCamera.Follow = transform;
        TPSCamera.LookAt = lookAtTarget;

        groundLayer = LayerMask.GetMask("Cells");

        //Initialize
        FPSMode = false;
        UpdateSensibility();
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

    /// <summary>
    /// Ground check
    /// </summary>
    private void IsGrounded()
    {
        //Cast a ray down from player position
        Ray ray = new Ray(transform.position, Vector3.down);

        //If collides, is grounded
        if (Physics.Raycast(ray, 1.1f, groundLayer) && !isGrounded)
        {
            isGrounded = true;
            //TODO: Step sound on falling
            //GetComponentInChildren<PlayerAnimEvents>().FootStep();
        }
        else if (isGrounded)
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
        hCamInput = Input.GetAxisRaw("Mouse X");
        vCamInput = Input.GetAxisRaw("Mouse Y");
        //jumpInput = Input.GetButtonDown("Jump");
        fireInput = Input.GetButton("Fire1");
        cameraModeInput = Input.GetKeyDown(KeyCode.F);
    }

    /// <summary>
    /// Handle player movement and rotation
    /// </summary>
    private void Move()
    {
        //Player movement
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
        //Horizontal rotation
        if (hCamInput != 0)
        {
            rotation += hCamInput * rotSensibility * Time.deltaTime;
            //Normalize yRotation
            if (rotation > 360)
            {
                rotation -= 360;
            }
            else if (rotation < 0)
            {
                rotation += 360;
            }
        }
        //Horizontal rotation target
        orientation.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
        if(FPSMode)
        {
            //Rotate the player horizontally smoothly
            transform.rotation = orientation.rotation;
        }
        else
        {
            //Rotate the player horizontally smoothly
            transform.DORotate(orientation.rotation.eulerAngles, rotDelay, RotateMode.Fast).SetId("CamRot");
        }

        //Vertical rotation
        if (vCamInput != 0)
        {
            float newLookAt = Mathf.Clamp(lookAtTarget.localPosition.y + vCamInput * (rotSensibility / 10) * Time.deltaTime, -2, 2);
            lookAtTarget.localPosition = new Vector3(0, newLookAt, 5);
        }
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
        //CameraHolder.instance.SwitchToTPS();
        rotDelay = 0.25f;
        FPSCamera.Priority = 9;
        HudController.instance.SwitchToTPS();
    }


    /// <summary>
    /// Switch to the FPS camera
    /// </summary>
    public void SwitchToFPS()
    {
        FPSMode = true;
        //playerMesh.SetActive(false);
        //CameraHolder.instance.SwitchToFPS();
        rotDelay = 0;
        FPSCamera.Priority = 11;
        HudController.instance.SwitchToFPS();
    }

    /// <summary>
    /// Public function to receive attacks damage
    /// </summary>
    /// <param name="damage"></param>
    public void ReceiveDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
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

    /// <summary>
    /// Start the character
    /// </summary>
    public void StartGame()
    {
        isGameStarted = true;
    }

    public void UpdateSensibility()
    {
        if (PlayerPrefs.HasKey("Sensibility")){
            rotSensibility = PlayerPrefs.GetFloat("Sensibility");
        }
    }

    #endregion
}
