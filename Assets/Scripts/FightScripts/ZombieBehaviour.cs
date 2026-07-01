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

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private HealthUniversal playerHealth;
    private bool isPursuing = false;
    private float lastAttackTime;
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
    }

  
    void Update()
    {
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
            }
            lastAttackTime = Time.time;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Красный круг — зона атаки

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRad);   // Желтый круг — зона видимости
    }
}
