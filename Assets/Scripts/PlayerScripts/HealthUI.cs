using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public HealthUniversal playerHealth;
    public Image fillImage;

    void Update()
    {
        fillImage.fillAmount =
            (float)playerHealth.currentHealth / playerHealth.maxHealth;
    }
}