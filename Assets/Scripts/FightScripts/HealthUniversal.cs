using UnityEngine;
using UnityEngine.SceneManagement;
public class HealthUniversal : MonoBehaviour
{
    [Header("��������� ��������")]
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
        
        Debug.Log("������");
        if (gameObject.CompareTag("Enemy"))
        {
            
            Destroy(gameObject);
        }
        else
        {
            GameOverMenuManager gameOver = FindAnyObjectByType<GameOverMenuManager>();

            if (gameOver != null)
            {
                gameOver.ShowGameOver();
            }
        }
            

            
        
    }

    private void RestartCurrentScene()
    {
        // ���� ������ ������������ ����� �� �����
        SwitchMagic switchMagic = FindFirstObjectByType<SwitchMagic>();

        if (switchMagic != null)
        {
            // ���� ������� �������� �������� ����� �� ��������� ������ �������
            switchMagic.LoadProgress(resetToLevelStart: true);
        }

        // ������������� �����
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
