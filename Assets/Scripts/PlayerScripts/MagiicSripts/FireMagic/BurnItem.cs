using UnityEngine;
using System.Collections;
public class BurnItem : MonoBehaviour, IFireInteractable
{
    [Header("Ссылки")]
    public AudioSource audioSource;
    public AudioClip fireClip;
    public ParticleSystem firePartical;

    [Header("Настройки магии")]       
    public float fireDuration = 3f;   // Длительность горения (сек)


    private bool isFire = false;


    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void OnFire()
    {
        if (isFire) return;

      if(audioSource  != null && fireClip != null)
        {
            audioSource.clip = fireClip;
            audioSource.Play();
        }

        if (firePartical != null)
        {
            firePartical.Play();
        }

        StartCoroutine(FireRoutine());

    }

    private IEnumerator FireRoutine()
    {
        isFire = true;
        yield return new WaitForSeconds(fireDuration);

        if (firePartical != null)
        {
            // Отвязываем пламя от родителя, чтобы Destroy объекта его не уничтожил
            firePartical.transform.parent = null;

            // Говорим пламени плавно остановиться (догореть)
            firePartical.Stop();

            // Говорим системе частиц самой удалиться из памяти, как только все искры исчезнут
            var mainModule = firePartical.main;
            mainModule.stopAction = ParticleSystemStopAction.Destroy;
        }

        Destroy(gameObject);

    }
}
