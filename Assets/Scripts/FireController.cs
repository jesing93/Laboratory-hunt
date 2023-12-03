using DigitalRuby.PyroParticles;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [Tooltip("Optional audio source to play once when the script starts.")]
    public AudioSource audioSource;

    [Tooltip("How long the script takes to fully start. This is used to fade in animations and sounds, etc.")]
    [SerializeField]
    private float startTime = 1.0f;

    [Tooltip("How long the script takes to fully stop. This is used to fade out animations and sounds, etc.")]
    [SerializeField]
    private float stopTime = 1.0f;

    [Tooltip("How long the effect lasts. Once the duration ends, the script lives for StopTime and then the object is destroyed.")]
    [SerializeField]
    private float duration = 2.0f;

    private float startTimeMultiplier;
    private float startTimeIncrement;

    private float stopTimeMultiplier;
    private float stopTimeIncrement;

    private Transform anchor;
    private bool isFiring;

    public bool Starting { get; private set; }
    public float StartPercent { get; private set; }
    public bool Stopping { get; private set; }
    public float StopPercent { get; private set; }

    private void Start()
    {
        //Components
        anchor = PlayerController.instance.FirePoint;

        // precalculate so we can multiply instead of divide every frame
        stopTimeMultiplier = 1.0f / stopTime;
        startTimeMultiplier = 1.0f / startTime;

        // If we implement the ICollisionHandler interface, see if any of the children are forwarding
        // collision events. If they are, hook into them.
        ICollisionHandler handler = (this as ICollisionHandler);
        if (handler != null)
        {
            FireCollisionForward collisionForwarder = GetComponentInChildren<FireCollisionForward>();
            if (collisionForwarder != null)
            {
                collisionForwarder.CollisionHandler = handler;
            }
        }
    }
    private void Update()
    {
        //Stick to the fire point
        transform.position = anchor.position;
        transform.rotation = anchor.rotation;
            
        // reduce the duration
        duration -= Time.deltaTime;
        if (Stopping)
        {
            // increase the stop time
            stopTimeIncrement += Time.deltaTime;
            if (stopTimeIncrement < stopTime)
            {
                StopPercent = stopTimeIncrement * stopTimeMultiplier;
            }
        }
        else if (Starting)
        {
            // increase the start time
            startTimeIncrement += Time.deltaTime;
            if (startTimeIncrement < startTime)
            {
                StartPercent = startTimeIncrement * startTimeMultiplier;
            }
            else
            {
                Starting = false;
            }
        }
        else if (duration <= 0.0f)
        {
            // time to stop, no duration left
            Stop();
        }
    }

    private void StartParticleSystems()
    {
        foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            if (p.main.startDelay.constant == 0.0f)
            {
                // wait until next frame because the transform may change
                var m = p.main;
                var d = p.main.startDelay;
                d.constant = 0.01f;
                m.startDelay = d;
            }
            p.Play();
        }
    }

    /// <summary>
    /// Fire function
    /// </summary>
    public void Fire()
    {
        duration = 1.0f;
        //If wasn't firing start effects
        if (!isFiring)
        {
            isFiring = true;
            Stopping = false;
            stopTimeIncrement = 0;
            StartParticleSystems();
            audioSource.Play();
        }
    }

    /// <summary>
    /// Stop effects function
    /// </summary>
    public virtual void Stop()
    {
        isFiring = false;
        audioSource.Stop();
        if (Stopping)
        {
            return;
        }
        Stopping = true;

        // cleanup particle systems
        foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            p.Stop();
        }

        StartCoroutine(CleanupEverythingCoRoutine());
    }
    private IEnumerator CleanupEverythingCoRoutine()
    {
        // 2 extra seconds just to make sure animation and graphics have finished ending
        yield return new WaitForSeconds(stopTime + 2.0f);

        if (!isFiring)
        {
            foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
            {
                p.Stop();
            }
            Stopping = false;
        }
    }
}
