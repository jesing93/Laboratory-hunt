using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    //Components
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] steps;
    private float lastStep;
    private readonly float stepDelay = 0.1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void FootStep()
    {
        if(lastStep + stepDelay < Time.time)
        {
            lastStep = Time.time;
            float currentSpeed = PlayerController.instance.Rb.velocity.magnitude;
            AudioClip clip = steps[Random.Range(0, steps.Length)];
            audioSource.pitch = Mathf.Clamp(currentSpeed / 5, .6f, 1.3f);
            audioSource.PlayOneShot(clip);
        }
    }
}
