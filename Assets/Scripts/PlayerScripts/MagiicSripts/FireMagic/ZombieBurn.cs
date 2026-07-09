using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieBurn : MonoBehaviour, IFireInteractable
{
    [Header("Ссылки")]
    public HealthUniversal zombieHealth;
    public NavMeshAgent navMeshAgent;
    public ParticleSystem fireParticle;
    private Animator animator;

    [Header("Настройки магии")]
    public float damage = 25f;          // Урон от заклинания
    public float fireDistance = 3f;   // Дальность перехода огня
    public float fireDuration = 4f;     // Длительность горения
    public float fireDamage = 5f; //урон от горения
    public float spreadInterval = 1f;

    private bool isBurning = false;
    private Coroutine burnCoroutine;
    private float elapsedTime = 0f;

    void Start()
    {
        if (zombieHealth == null) zombieHealth = GetComponent<HealthUniversal>();
        animator = GetComponent<Animator>();
        if (fireParticle != null && fireParticle.isPlaying)
        {
            fireParticle.Stop();
        }
    }

    
    void Update()
    {
        
    }



    public void OnFire()
    {
        // Первоначальный урон (25) теперь наносится ВСЕГДА при каждом попадании магии
        if (zombieHealth != null)
        {
            zombieHealth.TakeDamage(damage);
            if (animator != null) animator.SetTrigger("Hit");
        }

        if (isBurning)
        {
            // Если уже горит, просто сбрасываем время горения в начало (продлеваем эффект)
            // Корутину НЕ перезапускаем, чтобы не сбивать WaitForSeconds
            elapsedTime = 0f;
        }
        else
        {
            burnCoroutine = StartCoroutine(FireRoutine());
        }
    }

    private IEnumerator FireRoutine()
    {
        isBurning = true;

        if (fireParticle != null && !fireParticle.isPlaying)
        {
            fireParticle.Play();
        }

        elapsedTime = 0f;
        float spreadTimer = 0f;

        // Цикл работает, пока накопленное время меньше длительности горения
        while (elapsedTime < fireDuration)
        {
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
            spreadTimer += 1f;

            // Наносим периодический урон (5)
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(fireDamage);
                if (animator != null) animator.SetTrigger("Hit");
            }

            // Пытаемся поджечь соседей
            if (spreadTimer >= spreadInterval)
            {
                SpreadFire();
                spreadTimer = 0f;
            }
        }

        ExtinguishFire();
    }
    private void SpreadFire()
    {
        // Ищем все коллайдеры в радиусе fireDistance
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, fireDistance);

        foreach (var hitCollider in hitColliders)
        {
            // Проверяем, чтобы зомби не поджигал сам себя
            if (hitCollider.gameObject == this.gameObject) continue;

            // Если у соседа есть интерфейс IFireInteractable, поджигаем его
            if (hitCollider.TryGetComponent(out IFireInteractable neighbor))
            {
                neighbor.OnFire();
            }
        }
    }

    private void ExtinguishFire()
    {
        isBurning = false;

        if (fireParticle != null)
        {
            fireParticle.Stop();
        }
    }

    // Визуализация радиуса поражения в редакторе Unity при выделении зомби
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fireDistance);
    }

}
