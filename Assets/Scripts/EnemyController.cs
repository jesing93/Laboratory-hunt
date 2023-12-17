using DG.Tweening;
using MimicSpace;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyController : MonoBehaviour
{
    #region parameters

    //Components
    private MimicController myMimic;
    private NavMeshAgent navAgent;
    private StateMachine brain;
    [SerializeField]
    private GameObject eggPref;
    [SerializeField]
    private GameObject burningFX;
    private AudioSource mimicAudio;
    [SerializeField]
    private AudioClip[] mimicClips;
    [SerializeField]
    private AudioClip mimicDeathClip;

    //Vars
    private float takingFireDelay;
    private float onFireDelay;
    private float currentHealth;
    private float maxHealth;
    private float timeAlive;
    private int growStage;
    private bool isOnFire;
    private bool isAlive;
    private bool playerInSight;
    private float lastPlayerSight;
    private float roamRange = 100f;
    private int remainingEggs = 2;
    private float eggDelay;
    private float attackTimer = 2f;
    private float soundDelay;

    private LayerMask groundLayer;
    private LayerMask playerLayer;

    #endregion
    #region methods

    private void Awake()
    {
        //Components
        myMimic = GetComponentInChildren<MimicController>();
        navAgent = GetComponent<NavMeshAgent>();
        mimicAudio = GetComponent<AudioSource>();

        groundLayer = LayerMask.GetMask("Cells");
        playerLayer = LayerMask.GetMask("Player");

        //Initialize
        currentHealth = maxHealth = 50;
        timeAlive = 0;
        growStage = 1;

        brain = GetComponent<StateMachine>();
    }

    private void Update()
    {
        if (isAlive)
        {
            // Assigning velocity to the mimic to assure great leg placement
            myMimic.velocity = navAgent.velocity;

            HandleSight();
            HandleGowth();
            HandleLife();
            HandleSounds();

            //If map falling: kill
            if (transform.position.y <= 5000)
            {
                //currentHealth = 0;
            }
        }
    }

    /// <summary>
    /// Start the brain
    /// </summary>
    public void Init()
    {
        isAlive = true;
        brain.PushState(OnRoam, OnRoamEnter, OnRoamExit);
    }

    /// <summary>
    /// Detect if the player is in the line of sight
    /// </summary>
    private void HandleSight()
    {
        //playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        Ray ray = new(transform.position, PlayerController.instance.transform.position - transform.position);
        RaycastHit hit;
        Debug.DrawRay(transform.position, PlayerController.instance.transform.position - transform.position);
        if(Physics.Raycast(ray, out hit, 5f, groundLayer | playerLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                playerInSight = true;
                //Set last sight time + sight delay
                lastPlayerSight = Time.time +  3f;
            }else
            {
                playerInSight = false;
            }
        }
        else
        {
            playerInSight = false;
        }
    }

    /// <summary>
    /// Handle the generic roam behaviour
    /// </summary>
    private void HandleRoam()
    {
        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(transform.position, roamRange, false, out point))
            {
                //Debug.DrawRay(point, Vector3.up, Color.red, 2f);
                navAgent.SetDestination(point);
            }
        }
    }

    /// <summary>
    /// Generate a random reachable point in radius
    /// </summary>
    /// <param name="center">The center of the search</param>
    /// <param name="range">The max distance in Unity units</param>
    /// <param name="scape">True if the point should be far from here, minimum distance = range/2</param>
    /// <param name="result">The resulting vector3, current position if none was found</param>
    /// <returns></returns>
    private bool RandomPoint(Vector3 center, float range, bool scape, out Vector3 result)
    {
        //Counter to avoid infinite loop
        int failN = 0;
        Vector3 spherePoint = Vector3.zero;
        while (true)
        {
            if(scape)
            {
                //Random point with minimum distance
                spherePoint = new(Random.Range(0.5f, 1f) * Random.Range(0, 2) * 2 - 1, Random.Range(0.5f, 1f) * Random.Range(0, 2) * 2 - 1, Random.Range(0.5f, 1f) * Random.Range(0, 2) * 2 - 1);
            }
            else
            {
                //Random point
                spherePoint = Random.insideUnitSphere;
            }
            Vector3 randomPoint = center + spherePoint * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 50f, NavMesh.AllAreas))
            {
                //If valid point, return
                result = hit.position;
                return true;
            }
            failN++;
            if (failN > 10)
            {
                //If too many invalid points, return empty
                result = center;
                return false;
            }
        }
    }

    /// <summary>
    /// Handle taking fire damage or regenerationg life
    /// </summary>
    private void HandleLife()
    {
        //If alive
        if (currentHealth > 0)
        {
            if (takingFireDelay > 0) //If touching fire
            {
                takingFireDelay -= Time.deltaTime;
                currentHealth -= Time.deltaTime * 10;
                onFireDelay = 2f;
            }
            else if (onFireDelay > 0) //If burning
            {
                onFireDelay -= Time.deltaTime;
                currentHealth -= Time.deltaTime * 5; 
            }
            else if (currentHealth < maxHealth) //If not being damaged and not full health
            {
                if (isOnFire)
                {
                    isOnFire = false;
                    burningFX.SetActive(false);
                }
                currentHealth = Mathf.Clamp(currentHealth + Time.deltaTime * growStage, 0, maxHealth);
            }
        }
        else //Death event
        {
            StartCoroutine(DeathSequence());
        }
    }

    /// <summary>
    /// Handle mimic death
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeathSequence()
    {
        isAlive = false;
        Destroy(brain); 
        mimicAudio.clip = mimicDeathClip;
        mimicAudio.Play();
        myMimic.Die();
        myMimic.transform.DOScale(0.1f, 2).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1);
        myMimic.transform.DOLocalMoveY(0.1f, 1).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1.5f);
        GameManager.instance.KillEnemy(this.gameObject);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Handle the growth once enougth time alive
    /// </summary>
    private void HandleGowth()
    {
        timeAlive += Time.deltaTime;
        if (growStage < 3 && timeAlive > growStage * 10)
        {
            growStage++;
            maxHealth = 50 + 25 * growStage;
            //Grow phisical params
            //TODO: Improve growing
            myMimic.Grow(growStage);
            myMimic.newLegRadius = growStage/2;
            myMimic.minLegDistance = growStage/2;
            myMimic.partsPerLeg = growStage + 1;
            //height = 0.3f * growStage;
            myMimic.RegenerateLegStats();
        }
    }

    /// <summary>
    /// Handle the reproduction of several random sounds
    /// </summary>
    private void HandleSounds()
    {
        soundDelay -= Time.deltaTime;
        if(soundDelay < 0)
        {
            soundDelay = Random.Range(10f, 20f);
            mimicAudio.clip = mimicClips[Random.Range(0, mimicClips.Length - 1)];
            mimicAudio.Play();
        }

    }

    /// <summary>
    /// Handle the placement of eggs
    /// </summary>
    private void HandleLayEgg()
    {
        if(eggDelay < Time.time)
        {
            remainingEggs--;
            eggDelay = Time.time + 10f;
            Instantiate(eggPref, transform.position, transform.rotation);
        }
    }

    /// <summary>
    /// Handle particle collision with fire particles
    /// </summary>
    /// <param name="other"></param>
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Fire"))
        {
            takingFireDelay = 1f;
            if (!isOnFire)
            {
                isOnFire = true;
                burningFX.SetActive(true);
                foreach (ParticleSystem item in burningFX.GetComponentsInChildren<ParticleSystem>())
                {
                    item.Play();
                };
            }
        }
    }

    #endregion

    #region states

    private void OnRoamEnter()
    {
        navAgent.ResetPath();
    }

    /// <summary>
    /// Generic roaming function for every growth stage
    /// </summary>
    private void OnRoam()
    {
        switch (growStage)
        {
            case 1:
                if (playerInSight)
                {
                    brain.PushState(OnRunAway, OnRunAwayEnter, null);
                }
                else
                {
                    HandleRoam();
                }
                break;
            case 2:
                if (playerInSight)
                {
                    brain.PushState(OnRunAway, OnRunAwayEnter, null);
                } else
                {
                    if(remainingEggs > 0)
                    {
                        brain.PushState(OnLayEggs, null, null);
                    }
                    else
                    {
                        HandleRoam();
                    }
                }
                break;
            case 3:
                if (playerInSight)
                {
                    brain.PushState(OnChase, OnChaseEnter, OnChaseExit);
                }
                else
                {
                    if (remainingEggs > 0)
                    {
                        brain.PushState(OnLayEggs, null, null);
                    }
                    else
                    {
                        HandleRoam();
                    }
                }
                break;
        }
    }

    private void OnRoamExit()
    {
        
    }

    /// <summary>
    /// Simmilar to roaming but running to a far point
    /// </summary>
    private void OnRunAwayEnter()
    {
        navAgent.ResetPath();
    }

    /// <summary>
    /// Simmilar to roaming but running to a far point
    /// </summary>
    private void OnRunAway()
    {
        if (!playerInSight && lastPlayerSight <= Time.time || growStage > 2)
        {
            brain.PopState();
        }
        else 
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                Vector3 point;
                if (RandomPoint(transform.position, roamRange, true, out point))
                {
                    Debug.DrawRay(point, Vector3.up, Color.red, 2f);
                    navAgent.SetDestination(point);
                }
            }
        }
    }

    /// <summary>
    /// Roaming while searching for places to place eggs
    /// </summary>
    private void OnLayEggs()
    {
        if (playerInSight)
        {
            if (growStage > 2)
            {
                brain.PushState(OnChase, OnChaseEnter, OnChaseExit);
            }
            else
            {
                brain.PushState(OnRunAway, OnRunAwayEnter, null);
            }
        }
        else
        {
            HandleRoam();
            if (remainingEggs == 0)
            {
                brain.PopState();
            }
            else
            {
                Ray rayF = new(transform.position, transform.forward);
                Ray rayR = new(transform.position, transform.right);
                Ray rayL = new(transform.position, -transform.right);
                RaycastHit hit;
                int hitCounts = 0;
                if (Physics.Raycast(rayF, out hit, 0.5f, groundLayer))
                {
                    hitCounts++;
                }
                if (Physics.Raycast(rayR, out hit, 0.5f, groundLayer))
                {
                    hitCounts++;
                }
                if (Physics.Raycast(rayL, out hit, 0.5f, groundLayer))
                {
                    hitCounts++;
                }
                if (hitCounts > 1)
                {
                    HandleLayEgg();
                }
            }
        }
    }

    private void OnChaseEnter()
    {
        navAgent.ResetPath();
    }

    /// <summary>
    /// Following the player to attack
    /// </summary>
    private void OnChase()
    {
        navAgent.SetDestination(PlayerController.instance.transform.position);
        if(playerInSight && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < 2f)
        {
            brain.PushState(OnAttack, OnAttackEnter, OnAttackExit);
        }
        else if(!playerInSight && lastPlayerSight < Time.time)
        {
            brain.PopState();
        }
    }

    private void OnChaseExit()
    {
        //TODO: End chase sound
    }

    private void OnAttackEnter()
    {
        navAgent.ResetPath();
    }

    /// <summary>
    /// Attack sequence
    /// </summary>
    private void OnAttack()
    {
        attackTimer-= Time.deltaTime;
        if(Vector3.Distance(transform.position, PlayerController.instance.transform.position) > 5f)
        {
            brain.PopState();
        }
        else if(attackTimer <= 0)
        {
            //TODO: Attack anim
            PlayerController.instance.ReceiveDamage(20);
            attackTimer = 2f;
            brain.PopState();
        }
    }

    private void OnAttackExit()
    {

    }

    #endregion
}
