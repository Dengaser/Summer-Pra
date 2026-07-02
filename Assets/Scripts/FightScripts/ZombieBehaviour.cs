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
    public AudioClip[] growlSounds;     // Массив для случайных рыков
    public AudioClip attackSound;       // Звук атаки
    [SerializeField] private float minGrowlInterval = 5f; // Минимальное время между рыками
    [SerializeField] private float maxGrowlInterval = 12f; // Максимальное время между рыками
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

        //audioSource = GetComponent<AudioSource>();

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
        // Если пришло время рычать и в массиве есть звуки
        if (Time.time >= nextGrowlTime && growlSounds != null && growlSounds.Length > 0)
        {
            // Выбираем случайный индекс из массива
            int randomIndex = Random.Range(0, growlSounds.Length);

            if (growlSounds[randomIndex] != null)
            {
                // PlayOneShot позволяет звукам накладываться друг на друга и не прерывать текущие
                audioSource.PlayOneShot(growlSounds[randomIndex]);
            }

            // Рассчитываем время для следующего рыка
            CalculateNextGrowlTime();
        }
    }

    private void CalculateNextGrowlTime()
    {
        // Текущее время игры + случайный промежуток
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
