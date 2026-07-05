using UnityEngine;

public class InfinityBurnItem : MonoBehaviour, IFireInteractable
{

    [Header("—сылки")]
    public AudioSource audioSource;
    public AudioClip fireClip;
    public ParticleSystem firePartical;

    public void OnFire()
    {

        

        if (audioSource != null && fireClip != null)
        {
            audioSource.clip = fireClip;
            audioSource.Play();
        }

        if (firePartical != null)
        {
            firePartical.Play();
        }
    }

    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
}
