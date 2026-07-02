using UnityEngine;

public class SlotZone : MonoBehaviour
{
    [SerializeField] private string targetTag = "Box"; 
    private bool isOccupied = false;

    public bool IsOccupied => isOccupied;

    // Временный метод для теста физики без ограничений
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[ТЕСТ КОЛЛИЗИИ] Коробка коснулась твердого коллайдера отверстия: {collision.gameObject.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Лог БЕЗ проверки тега (сработает на ЛЮБОЙ объект, даже на игрока)
        Debug.Log($"<color=yellow>[SlotZone ТЕСТ]</color> Что-то вошло в триггер отверстия: {other.name} (Тег объекта: {other.tag})");

        // 2. Стандартная проверка
        if (other.CompareTag(targetTag))
        {
            isOccupied = true;
            Debug.Log($"<color=green>[SlotZone]</color> УСПЕХ: Объект {other.name} с тегом {targetTag} встал в пазы.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isOccupied = false;
        }
    }
}