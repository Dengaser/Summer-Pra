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

    [Header("Настройки здоровья зомби")]
    public float maxHealth = 50f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Настройки звуков")]
    public AudioClip[] growlSounds;
    public AudioClip attackSound;
    [SerializeField] private float minGrowlInterval = 5f;
    [SerializeField] private float maxGrowlInterval = 12f;
    public AudioSource audioSource;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private HealthUniversal playerHealth;
    private Animator animator; // Ссылка на аниматор

    private bool isPursuing = false;
    private float lastAttackTime;
    private float nextGrowlTime;

    void Start()
    {
        currentHealth = maxHealth;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = stoppingDistance;
        animator = GetComponent<Animator>(); // Получаем компонент аниматора

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
        if (isDead) return; // Если мертв, ничего не делаем

        HandleGrowling();

        if (player == null)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (!isPursuing)
        {
            if (distanceToPlayer <= visionRad)
            {
                isPursuing = true;
                Debug.Log("заметил");
            }
            // Если не преследует, значит стоит в Idle
            animator.SetBool("isWalking", false);
        }
        else
        {
            navMeshAgent.SetDestination(player.position);

            // ИСПРАВЛЕНО: Проверяем скорость агента, чтобы включить ходьбу
            bool isMoving = navMeshAgent.velocity.sqrMagnitude > 0.1f;
            animator.SetBool("isWalking", isMoving);

            if (distanceToPlayer <= attackRange)
            {
                TryAttack();
            }
            // ... дальше твой код без изменений
        }
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Запускаем триггер атаки в аниматоре
            animator.SetTrigger("Attack");

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);

                if (attackSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(attackSound);
                }
            }
            lastAttackTime = Time.time;
        }
    }

    // Этот метод должны вызывать другие скрипты (например, пуля или меч игрока), чтобы нанести урон зомби
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Если выжил — проигрываем анимацию получения урона
            animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        isDead = true;
        navMeshAgent.isStopped = true; // Останавливаем зомби
        animator.SetBool("isDead", true); // Включаем анимацию смерти

        // Отключаем коллайдер, чтобы мертвый зомби не мешал игроку ходить
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log("Зомби уничтожен");
        Destroy(gameObject, 5f); // Удаляем труп через 5 секунд
    }

    private void HandleGrowling()
    {
        if (Time.time >= nextGrowlTime && growlSounds != null && growlSounds.Length > 0 && audioSource != null)
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
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRad);
    }
}