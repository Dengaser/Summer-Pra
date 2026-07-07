using UnityEngine;
using UnityEngine.SceneManagement;
public class HealthUniversal : MonoBehaviour
{
    [Header("Настройки значений")]
    public float maxHealth = 100;

    private Animator animator;



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
        if (gameObject.CompareTag("Enemy"))
        {
            
            Destroy(gameObject);
        }
        else
        {
            RestartCurrentScene();
        }
            

            
        
    }

    private void RestartCurrentScene()
    {
        // Ищем скрипт переключения магии на сцене
        SwitchMagic switchMagic = FindFirstObjectByType<SwitchMagic>();

        if (switchMagic != null)
        {
            // Даем команду откатить прогресс магий до состояния «старт уровня»
            switchMagic.LoadProgress(resetToLevelStart: true);
        }

        // Перезапускаем сцену
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
