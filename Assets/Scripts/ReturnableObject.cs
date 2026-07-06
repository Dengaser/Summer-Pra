using UnityEngine;

public class ReturnableObject : MonoBehaviour
{
    [Header("Настройки целевой позиции:")]
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 targetRotationEuler;

    [Header("Использовать глобальные координаты?")]
    [SerializeField] private bool useGlobalCoordinates = true;

    /// <summary>
    /// Метод для возврата объекта на заранее заданную позицию.
    /// </summary>
    public void ResetToTargetPosition()
    {
        if (useGlobalCoordinates)
        {
            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(targetRotationEuler);
        }
        else
        {
            transform.localPosition = targetPosition;
            transform.localRotation = Quaternion.Euler(targetRotationEuler);
        }

        Debug.Log($"Объект {gameObject.name} успешно вернулся на исходную позицию!");
    }

    // Кнопка в контекстном меню компонента (три точки в инспекторе), 
    // чтобы быстро скопировать текущую позицию объекта как целевую
    [ContextMenu("Запомнить текущую позицию")]
    private void SaveCurrentPosition()
    {
        if (useGlobalCoordinates)
        {
            targetPosition = transform.position;
            targetRotationEuler = transform.rotation.eulerAngles;
        }
        else
        {
            targetPosition = transform.localPosition;
            targetRotationEuler = transform.localRotation.eulerAngles;
        }
    }
}