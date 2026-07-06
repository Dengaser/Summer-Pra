using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [Header("Двери (должны иметь скрипт MovingDoor)")]
    [SerializeField] private MovingDoor doorToOpen;
    [SerializeField] private MovingDoor doorToClose;

    [Header("Настройки триггера")]
    [SerializeField] private string targetTag = "Cube";

    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && !isPressed)
        {
            isPressed = true;
            UpdateDoors();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag) && isPressed)
        {
            isPressed = false;
            UpdateDoors();
        }
    }

    private void UpdateDoors()
    {
        if (isPressed)
        {
            // Кнопка нажата: первую дверь открываем (true), вторую закрываем (false)
            if (doorToOpen != null) doorToOpen.SetOpen(true);
            if (doorToClose != null) doorToClose.SetOpen(false);
        }
        else
        {
            // Кнопка отпущена: возвращаем всё назад
            if (doorToOpen != null) doorToOpen.SetOpen(false);
            if (doorToClose != null) doorToClose.SetOpen(true);
        }
    }
}