using UnityEngine;

public class PuzzlePipeNode2 : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Настройки вращения")]
    [Tooltip("Выберите ось, вокруг которой будет вращаться элемент")]
    public RotationAxis rotationAxis = RotationAxis.Y;
    public float rotationSpeed = 10f;

    private float targetRotationAngle;

    // Ссылка на правильную (вторую) версию менеджера
    private PuzzleManager2 manager;

    // Переменные для хранения фиксированных начальных углов
    private float initialX;
    private float initialY;
    private float initialZ;

    private void Start()
    {
        // Жестко сохраняем начальные углы один раз, чтобы избежать «скачков» кватернионов Unity в Update
        initialX = transform.localEulerAngles.x;
        initialY = transform.localEulerAngles.y;
        initialZ = transform.localEulerAngles.z;

        // Выбираем стартовый угол в зависимости от оси
        float startingAngle = rotationAxis == RotationAxis.X ? initialX :
                              rotationAxis == RotationAxis.Y ? initialY :
                                                               initialZ;

        // Округляем до ближайших 90 градусов
        targetRotationAngle = Mathf.Round(startingAngle / 90f) * 90f;

        // Ищем обновленный менеджер на сцене
        manager = FindFirstObjectByType<PuzzleManager2>();
    }

    private void Update()
    {
        Quaternion targetRotation = transform.localRotation;

        // Вычисляем целевое вращение, меняя ТОЛЬКО выбранную ось
        switch (rotationAxis)
        {
            case RotationAxis.X:
                targetRotation = Quaternion.Euler(targetRotationAngle, initialY, initialZ);
                break;
            case RotationAxis.Y:
                targetRotation = Quaternion.Euler(initialX, targetRotationAngle, initialZ);
                break;
            case RotationAxis.Z:
                targetRotation = Quaternion.Euler(initialX, initialY, targetRotationAngle);
                break;
        }

        // Плавно вращаем объект к целевому углу
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // Этот метод должен вызываться при клике/взаимодействии с трубой
    public void RotateNode()
    {
        targetRotationAngle = (targetRotationAngle + 90f) % 360f;

        if (manager != null)
        {
            manager.CheckPuzzleDelayed();
        }
        else
        {
            Debug.LogError($"На сцене не найден PuzzleManager2, объект {name} не может сообщить о повороте!");
        }
    }

    // Проверяет, находится ли труба в указанном угле
    public bool IsAtAngle(float checkAngle)
    {
        float currentAngle = Mathf.Repeat(Mathf.Round(targetRotationAngle), 360f);
        float targetAngle = Mathf.Repeat(Mathf.Round(checkAngle), 360f);

        return Mathf.Abs(currentAngle - targetAngle) < 1f;
    }
}