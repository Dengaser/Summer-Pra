using UnityEngine;

public class Fire : MonoBehaviour , IIceInteractable
{
    [Header("Ёффекты")]
    public ParticleSystem fireParticle;
    public AudioSource fireSource;
    public AudioClip fireClip;

    [Header("»грок")]
    public string playerTag = "Player";
    private HealthUniversal playerHealth;

    [Header("”рон")]
    public float damage = 10f;

    public void OnFreeze()
    {
       
        if (fireParticle != null)
        {
            fireParticle.Stop();
        }

       
        if (fireSource != null)
        {
            fireSource.Stop();
        }

        
        if (TryGetComponent(out Collider fireCollider))
        {
            fireCollider.enabled = false;
        }

        
        Destroy(gameObject, 2.0f);
    }





    void Start()
    {
        
        if (fireSource != null && fireClip != null)
        {
            fireSource.clip = fireClip; 
            fireSource.Play();         
        }

        GameObject playerObg = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObg != null)
        {
            playerHealth = playerObg.GetComponent<HealthUniversal>();
        }
    }



}
