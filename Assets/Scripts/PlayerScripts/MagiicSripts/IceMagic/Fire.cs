using UnityEngine;

public class Fire : MonoBehaviour , IIceInteractable
{
    [Header("Эффекты")]
    public ParticleSystem fireParticle;
    public AudioSource fireSource;
    public AudioClip fireClip;

    [Header("Игрок")]
    public string playerTag = "Player";
    private HealthUniversal playerHealth;

    [Header("Урон")]
    public float damage = 10f;

    public void OnFreeze()
    {
        // 1. Останавливаем спавн новых частиц огня
        if (fireParticle != null)
        {
            fireParticle.Stop();
        }

        // 2. Выключаем звук пламени
        if (fireSource != null)
        {
            fireSource.Stop();
        }

        // 3. Отключаем коллайдер, чтобы игрок больше не горел, пока частицы догорают
        if (TryGetComponent(out Collider fireCollider))
        {
            fireCollider.enabled = false;
        }

        // 4. Уничтожаем объект огня. 
        // Передаем 2.0f (или другое время), чтобы уже вылетевшие частицы красиво исчезли, а не пропали мгновенно
        Destroy(gameObject, 2.0f);
    }


   

  
    void Start()
    {
        GameObject playerObg = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObg != null)
        {
            playerHealth = playerObg.GetComponent<HealthUniversal>();
        }
    }

    
    
}
