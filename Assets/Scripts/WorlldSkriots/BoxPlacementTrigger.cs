using UnityEngine;
using UnityEngine.Events;

public class BoxPlacementTrigger : MonoBehaviour
{
    [Header("Ссылки на отверстия (Слоты)")]
    [SerializeField] private SlotZone[] slots;

    [Header("Что произойдет при успехе")]
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private ChapterEndController chapterEndController; // <-- ДОБАВИЛИ: Ссылка на скрипт финала главы
    [SerializeField] private UnityEvent onPuzzleComplete;

    [Header("Если условия не выполнены")]
    [SerializeField] private UnityEvent onActivationFailed;

    private bool isSolved = false;

    public void TryActivate()
    {
        if (isSolved) return;

        if (CheckAllSlots())
            ActivateTarget();
        else
            OnFailed();
    }

    private bool CheckAllSlots()
    {
        if (slots == null || slots.Length == 0) return false;

        foreach (SlotZone slot in slots)
        {
            if (slot == null || !slot.IsOccupied)
            {
                return false;
            }
        }
        return true;
    }

    private void ActivateTarget()
    {
        isSolved = true;
        Debug.Log("Попытка запуска успешна! Все ящики на месте.");

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        // <-- ДОБАВИЛИ: Запускаем потемнение экрана и надпись через код
        if (chapterEndController != null)
        {
            chapterEndController.TriggerChapterEnd();
        }

        onPuzzleComplete?.Invoke();
    }

    private void OnFailed()
    {
        Debug.Log("Запуск не удался: Не все ящики находятся в отверстиях!");
        onActivationFailed?.Invoke();
    }
}