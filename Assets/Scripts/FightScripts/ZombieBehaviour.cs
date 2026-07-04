using UnityEngine;
using UnityEngine.AI;

public class ZombieBehaviour : MonoBehaviour
{
    [Header("Настройки обнаружения")]
    public string playerTag = "Player";
    public float visionRad = 15f;
    public float stoppingDistance = 1.5f;

   

    [Header("Настройки атаки")]
    public float damage = 10f;          
    public float attackCooldown = 1.5f; 
    public float attackRange = 1.8f;


    [Header("Настройки звуков")]
    public AudioClip[] growlSounds;     
    public AudioClip attackSound;     
    [SerializeField] private float minGrowlInterval = 5f; 
    [SerializeField] private float maxGrowlInterval = 12f;
    public AudioSource audioSource;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private HealthUniversal playerHealth;
    private bool isPursuing = false;
    private float lastAttackTime;
    private float nextGrowlTime;
    void Start()
    {


        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = stoppingDistance;

      

        GameObject playerObg = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObg != null)
        {
            player = playerObg.transform;
            playerHealth = playerObg.GetComponent<HealthUniversal>();
        }

        CalculateNextGrowlTime();
    }

  
    void Update()
    {

        HandleGrowling();

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (!isPursuing)
        {
            if(distanceToPlayer<=visionRad)
            {
                isPursuing = true;
                Debug.Log("заметил");
            }
        }
        else
        {
            navMeshAgent.SetDestination(player.position);

            if(distanceToPlayer <= attackRange)
            {
                TryAttack();
            }
            if(distanceToPlayer > visionRad*1.5f)
            {
                isPursuing = false;
                navMeshAgent.ResetPath();
                Debug.Log("больше не видит");

            }
        }

    }


    private void TryAttack()
    {
        if(Time.time - lastAttackTime >= attackCooldown)
        {
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damage);

                if (attackSound != null)
                {
                    audioSource.PlayOneShot(attackSound);
                }
            }
            lastAttackTime = Time.time;
        }
    }



    private void HandleGrowling()
    {
       
        if (Time.time >= nextGrowlTime && growlSounds != null && growlSounds.Length > 0)
        {
          
            int randomIndex = Random.Range(0, growlSounds.Length);

            if (growlSounds[randomIndex] != null)
            {
                
                audioSource.PlayOneShot(growlSounds[randomIndex]);
            }

          
            CalculateNextGrowlTime();
        }
    }

    private void CalculateNextGrowlTime()
    {
      
        nextGrowlTime = Time.time + Random.Range(minGrowlInterval, maxGrowlInterval);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Красный круг — зона атаки

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRad);   // Желтый круг — зона видимости
    }
}
