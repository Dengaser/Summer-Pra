using UnityEngine;

public class HealthUniversal : MonoBehaviour
{
    [Header("Настройки значений")]
    public float maxHealth = 100;
    
  
  


    public float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }
    void Update()
    {

    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;
        
        currentHealth -= amount;
        
        if (currentHealth <= 0)
        {
            Death();

        }
    } 

    

    private void Death()
    {
        
            Debug.Log("смерть");
            Destroy(gameObject);
        
    }
}
