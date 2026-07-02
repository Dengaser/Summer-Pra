using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [Header("Настройки вращения моста")]
    public Vector3 targetRotation = new Vector3(0f, 0f, 0f); // Каким должен быть угол, когда мост опущен
    public float LoweringSpeed = 2f;

    private bool shouldLower = false;
    private Quaternion targetTargetQuaternion;

    private void Start()
    {
        targetTargetQuaternion = Quaternion.Euler(targetRotation);
    }

    private void Update()
    {
        if (shouldLower)
        {
            // Плавно опускаем мост к целевому повороту
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetTargetQuaternion, Time.deltaTime * LoweringSpeed);

            // Если почти долетел — отключаем апдейт для оптимизации
            if (Quaternion.Angle(transform.localRotation, targetTargetQuaternion) < 0.1f)
            {
                transform.localRotation = targetTargetQuaternion;
                shouldLower = false;
                Debug.Log("Мост полностью опущен.");
            }
        }
    }

    // Этот метод мы будем вызывать через UnityEvent в менеджере
    public void LowerBridge()
    {
        if (shouldLower) return;
        shouldLower = true;
        Debug.Log("Мост начинает опускаться...");
    }
}