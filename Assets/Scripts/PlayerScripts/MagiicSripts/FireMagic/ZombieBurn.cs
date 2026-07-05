using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieBurn : MonoBehaviour, IFireInteractable
{
    [Header("Ссылки")]
    public HealthUniversal zombieHealth;
    public NavMeshAgent navMeshAgent;
    public ParticleSystem fireParticle;

    [Header("Настройки магии")]
    public float damage = 25f;          // Урон от заклинания
    public float fireDistance = 3f;   // Дальность перехода огня
    public float fireDuration = 4f;     // Длительность горения
    public float fireDamage = 5f; //урон от горения
    public float spreadInterval = 1f;

    private bool isBurning = false;
    private Coroutine burnCoroutine;

    void Start()
    {
        if (zombieHealth == null) zombieHealth = GetComponent<HealthUniversal>();
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
        if (isBurning)
        {
            if (burnCoroutine != null) StopCoroutine(burnCoroutine);
        }
        else
        {
            // Наносим первоначальный урон только при первом поджоге
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(damage);
            }
        }

        burnCoroutine = StartCoroutine(FireRoutine());

    }

    private IEnumerator FireRoutine()
    {
        isBurning = true;

        if (fireParticle != null && !fireParticle.isPlaying)
        {
            fireParticle.Play();
        }

        float elapsed = 0f;
        float spreadTimer = 0f;

        // Цикл работает, пока не кончится время горения
        while (elapsed < fireDuration)
        {
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
            spreadTimer += 1f;

            // Наносим периодический урон
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(fireDamage);
            }

            // Каждую секунду (или с заданным интервалом) пытаемся поджечь соседей
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
