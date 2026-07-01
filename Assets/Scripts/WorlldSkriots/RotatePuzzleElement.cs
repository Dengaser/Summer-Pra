using UnityEngine;

public class RotatePuzzleElement : InteractableObject
{
    [Header("Связанный объект (для рычагов)")]
    [Tooltip("Если этот рычаг должен вращать другой провод, перетащи этот провод сюда. Если оставить пустым, объект будет вращать сам себя.")]
    public RotatePuzzleElement targetWire;

    [Header("Настройки вращения")]
    [Tooltip("Угол поворота при одном взаимодействии (в градусах)")]
    public float rotationAngle = 90f;

    [Tooltip("Скорость плавного вращения. Если 0, то поворот будет мгновенным.")]
    public float rotationSpeed = 10f;

    [Header("Текущее состояние цепи")]
    [Tooltip("Массив правильных углов поворота по оси Z, при которых этот элемент считается соединенным")]
    public float[] correctAngles = { 0f };

    private Quaternion targetRotation;
    private bool isRotating = false;

    void Start()
    {
        targetRotation = transform.localRotation;
    }

    void Update()
    {
        if (rotationSpeed > 0)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
            {
                transform.localRotation = targetRotation;
                isRotating = false;
            }
        }
    }

    // Метод вызывается из PlayerViewHelper при нажатии на E
    public new void OnInteract()
    {
        // Если этот объект — рычаг, управляющий другим проводом
        if (targetWire != null)
        {
            // Проигрываем анимацию/вращение самого рычага для визуала
            if (!isRotating || rotationSpeed == 0)
            {
                RotateSelf();
            }

            // И заставляем вращаться связанный провод
            targetWire.InteractFromLever();
        }
        else
        {
            // Если это обычный провод, он просто крутит сам себя
            if (isRotating && rotationSpeed > 0) return;
            RotateSelf();
        }
    }

    // Специальный метод, который вызовет рычаг у связанного провода
    public void InteractFromLever()
    {
        if (isRotating && rotationSpeed > 0) return;
        RotateSelf();
    }

    private void RotateSelf()
    {
        isRotating = true;

        if (rotationSpeed > 0)
        {
            targetRotation *= Quaternion.Euler(0, 0, rotationAngle);
        }
        else
        {
            transform.Rotate(0, 0, rotationAngle);
            targetRotation = transform.localRotation;
            CheckConnection();
        }
    }

    public bool CheckConnection()
    {
        float currentAngle = transform.localEulerAngles.z;
        currentAngle = Mathf.Round(currentAngle);
        if (currentAngle >= 360f) currentAngle -= 360f;
        if (currentAngle < 0f) currentAngle += 360f;

        foreach (float correctAngle in correctAngles)
        {
            float target = correctAngle;
            if (target >= 360f) target -= 360f;
            if (target < 0f) target += 360f;

            if (Mathf.Abs(currentAngle - target) < 2f)
            {
                return true;
            }
        }
        return false;
    }
}