using UnityEngine;

public class DoorController : MonoBehaviour
{
    public enum DoorType
    {
        Rotate, // Дверь открывается поворотом (на петлях)
        Slide   // Дверь открывается сдвигом (вверх/вбок)
    }

    [Header("Тип открытия двери")]
    public DoorType doorType = DoorType.Rotate;

    [Header("Настройки для Поворота (Rotate)")]
    [Tooltip("На сколько градусов повернуть дверь по осям относительно начального положения")]
    public Vector3 openRotationOffset = new Vector3(0f, 90f, 0f);

    [Header("Настройки для Сдвига (Slide)")]
    [Tooltip("На сколько единиц сдвинуть дверь относительно начального положения (например, Vector3.up * 3f)")]
    public Vector3 openPositionOffset = new Vector3(0f, 3f, 0f);

    [Header("Общие настройки")]
    public float openSpeed = 2f;

    private bool isOpening = false;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Start()
    {
        // Вычисляем конечные точки относительно стартового положения в сцене
        targetPosition = transform.localPosition + openPositionOffset;
        targetRotation = transform.localRotation * Quaternion.Euler(openRotationOffset);
    }

    private void Update()
    {
        if (isOpening)
        {
            if (doorType == DoorType.Rotate)
            {
                // Плавно поворачиваем дверь
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * openSpeed);

                // Прекращаем апдейт, когда дверь почти открылась
                if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
                {
                    transform.localRotation = targetRotation;
                    isOpening = false;
                    Debug.Log("Дверь полностью повернулась и открыта!");
                }
            }
            else if (doorType == DoorType.Slide)
            {
                // Плавно двигаем дверь
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, openSpeed * Time.deltaTime);

                // Прекращаем апдейт
                if (Vector3.Distance(transform.localPosition, targetPosition) < 0.001f)
                {
                    transform.localPosition = targetPosition;
                    isOpening = false;
                    Debug.Log("Дверь полностью сдвинулась и открыта!");
                }
            }
        }
    }

    // Этот метод вызывается через событие On Final Puzzle Solved () в PuzzleManager
    public void OpenDoor()
    {
        if (isOpening) return;
        isOpening = true;
        
        Debug.Log("Сигнал получен: Дверь начинает открываться...");
    }
}