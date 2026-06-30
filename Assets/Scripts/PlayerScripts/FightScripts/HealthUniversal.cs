using UnityEngine;

public class HealthUniversal : MonoBehaviour
{
    [Header("Настройки значений")]
    public float maxHealth = 100;
    public float damage = 10;
    

    private float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }
    void Update()
    {
        
    }

    private void Damage()
    {
        if(currentHealth > 0)
        {
            currentHealth -= damage;
        }
    }


}
