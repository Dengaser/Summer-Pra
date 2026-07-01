using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    [Header("Настройки открытия")]
    [Tooltip("На сколько градусов повернуть дверь при открытии (по оси Y)")]
    public float openAngle = 90f;

    [Tooltip("Скорость открытия двери")]
    public float openSpeed = 2f;

    private bool isOpening = false;
    private Quaternion targetRotation;

    private void Start()
    {
        // Вычисляем целевой поворот на основе начального положения
        targetRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + openAngle, transform.localEulerAngles.z);
    }

    private void Update()
    {
        if (isOpening)
        {
            // Плавно интерполируем текущий поворот к целевому
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * openSpeed);

            // Если дверь практически открылась, останавливаем обновление
            if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
            {
                transform.localRotation = targetRotation;
                isOpening = false;
                enabled = false; // Отключаем скрипт для оптимизации
            }
        }
    }

    // Этот метод мы будем вызывать из PuzzleManager
    public void Open()
    {
        if (isOpening) return;

        isOpening = true;
        Debug.Log("Дверь открывается!");
    }
}