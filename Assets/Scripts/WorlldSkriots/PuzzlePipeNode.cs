using UnityEngine;

public class PuzzlePipeNode : MonoBehaviour
{
    [Header("Настройки анимации")]
    public float rotationSpeed = 10f;

    private float targetYRotation;
    private PuzzleManager manager;

    private void Start()
    {
        // Округляем стартовый угол до ближайших 90 градусов
        targetYRotation = Mathf.Round(transform.localEulerAngles.y / 90f) * 90f;
        manager = FindFirstObjectByType<PuzzleManager>();
    }

    private void Update()
    {
        // Плавно вращаем объект к целевому углу
        Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles.x, targetYRotation, transform.localEulerAngles.z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void RotateNode()
    {
        targetYRotation = (targetYRotation + 90f) % 360f;

        if (manager != null)
        {
            manager.CheckPuzzleDelayed();
        }
    }

    // НОВЫЙ МЕТОД: Проверяет, находится ли труба в указанном угле (с округлением)
    public bool IsAtAngle(float checkAngle)
    {
        float currentAngle = Mathf.Repeat(Mathf.Round(targetYRotation), 360f);
        float targetAngle = Mathf.Repeat(Mathf.Round(checkAngle), 360f);

        return Mathf.Abs(currentAngle - targetAngle) < 1f;
    }
}