using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieFreeze : MonoBehaviour, IIceInteractable
{
    [Header("Ссылки")]
    public HealthUniversal zombieHealth;
    public NavMeshAgent navMeshAgent;
    private Animator animator;


    [Header("Настройки магии")]
    public float damage = 25f;          // Урон от заклинания
    public float freezeDuration = 3f;   // Длительность полной заморозки (сек)
    public float slowDuration = 4f;     // Длительность замедления после разморозки (сек)
    [Range(0f, 1f)]
    public float slowEffectiveness = 0.5f; // На сколько процентов замедлить (0.5 = на 50%)

    private bool isFrozen = false;
    private float originalSpeed;

   
    void Start()
    {

        if (zombieHealth == null) zombieHealth = GetComponent<HealthUniversal>();
        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();

        if (navMeshAgent != null)
        {
            originalSpeed = navMeshAgent.speed;
        }
    }

   
    void Update()
    {
        
    }


    public void OnFreeze()
    {
      
        if (zombieHealth != null)
        {
            zombieHealth.TakeDamage(damage);

            if (animator != null) animator.SetTrigger("Hit");

        }

        if (isFrozen)
        {
            StopAllCoroutines();
        }

        StartCoroutine(FreezeRoutine());
    }



    private IEnumerator FreezeRoutine()
    {
        isFrozen = true;

        // Если это первая заморозка, запоминаем нормальную скорость зомби
        // (на случай, если скорость меняется в других скриптах, берем текущую)
        if (navMeshAgent != null)
        {
            // Небольшая страховка: если агент уже стоял, не перезаписываем базовую скорость нулем
            if (navMeshAgent.speed > 0 && !isFrozen)
                originalSpeed = navMeshAgent.speed;
            else if (originalSpeed <= 0)
                originalSpeed = 3.5f; // Дефолтное значение NavMeshAgent, если что-то пошло не так

            // 1. Полная заморозка (скорость = 0)
            navMeshAgent.speed = 0f;
        }
        if (animator != null) animator.speed = 0f;
        // Ждем время полной заморозки
        yield return new WaitForSeconds(freezeDuration);

        if (animator != null) animator.speed = 1f;

        // 2. Разморозка и замедление
        if (navMeshAgent != null)
        {
            // Вычисляем пониженную скорость (например, 50% от изначальной)
            navMeshAgent.speed = originalSpeed * (1f - slowEffectiveness);
        }

        // Ждем время замедления
        yield return new WaitForSeconds(slowDuration);

        // 3. Возвращаем нормальную скорость
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = originalSpeed;
        }

        isFrozen = false;
    }
}
