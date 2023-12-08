using MimicSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Body Height from ground")]
    [Range(0.5f, 5f)]
    private float height = 0.4f;
    private float speed = 5f;
    Vector3 velocity = Vector3.zero;
    private float velocityLerpCoef = 4f;

    //Components
    private MimicController myMimic;

    //Vars
    private float takingFireDelay;
    private float onFireDelay;
    private float currentHealth;
    private float maxHealth;
    private float timeAlive;
    private int growStage;
    private bool isAlive;

    private LayerMask groundLayer;
    private LayerMask defaultLayer;

    private void Awake()
    {
        //Components
        myMimic = GetComponent<MimicController>();

        groundLayer = LayerMask.GetMask("Cells");
        defaultLayer = LayerMask.GetMask("Default");

        //Initialize
        isAlive = true;
        currentHealth = maxHealth = 50;
        timeAlive = 0;
        growStage = 1;
    }

    private void Update()
    {
        if (isAlive)
        {
            HandleMovement();
            HandleGowth();
            HandleLife();
        }
    }

    private void HandleMovement()
    {
        velocity = Vector3.Lerp(velocity, new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * speed, velocityLerpCoef * Time.deltaTime);

        // Assigning velocity to the mimic to assure great leg placement
        myMimic.velocity = velocity;

        transform.position = transform.position + velocity * Time.deltaTime;
        RaycastHit hit;
        Vector3 destHeight = transform.position;
        if (Physics.Raycast(transform.position + Vector3.up, -Vector3.up, out hit, 100, groundLayer | defaultLayer))
            destHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, destHeight, velocityLerpCoef * Time.deltaTime);
    }

    private void HandleLife()
    {
        if (currentHealth > 0)
        {
            if (takingFireDelay > 0)
            {
                takingFireDelay -= Time.deltaTime;
                currentHealth -= Time.deltaTime * 10;
                onFireDelay = 2f;
            }
            else if (onFireDelay > 0)
            {
                onFireDelay -= Time.deltaTime;
                currentHealth -= Time.deltaTime * 5;
            }
            else if (currentHealth < maxHealth)
            {
                currentHealth = Mathf.Clamp(currentHealth + Time.deltaTime * 2 * growStage, 0, maxHealth);
            }
        }
        else
        {
            isAlive = false;
            //TODO: Handle enemy death
            Debug.Log("Die!");
        }
    }

    private void HandleGowth()
    {
        timeAlive += Time.deltaTime;
        if (growStage < 3 && timeAlive > growStage * 10)
        {
            growStage++;
            maxHealth = 50 * growStage;
            //Grow phisical params
            myMimic.newLegRadius = growStage;
            myMimic.minLegDistance = growStage;
            myMimic.partsPerLeg = growStage + 1;
            height = 0.3f * growStage;
            myMimic.ResetMimic();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Fire"))
        {
            takingFireDelay = 1f;
        }
    }
}
