using UnityEngine;

public class BoxPlacementTrigger : MonoBehaviour
{
    [Header("Ссылки на 3 отверстия")]
    [SerializeField] private SlotZone[] slots;

    [Header("Ссылка на скрипт финала главы")]
    [SerializeField] private ChapterEndController chapterEndController;

    private bool isSolved = false;

    public void TryActivate()
    {
        Debug.Log($"<color=yellow>[BoxPlacementTrigger]</color> Вызван метод TryActivate на объекте {gameObject.name}. Начинаем проверку...");

        if (isSolved)
        {
            Debug.Log("[BoxPlacementTrigger] Головоломка уже была решена ранее.");
            return;
        }

        if (CheckAllSlots())
        {
            isSolved = true;
            Debug.Log("<color=cyan>[BoxPlacementTrigger] УСПЕХ!</color> Все 3 ящика на местах. Запускаем потемнение экрана.");

            if (chapterEndController != null)
            {
                chapterEndController.TriggerChapterEnd();
            }
            else
            {
                Debug.LogError("[BoxPlacementTrigger] Не назначена ссылка на ChapterEndController в инспекторе рычага!");
            }
        }
        else
        {
            Debug.Log("<color=orange>[BoxPlacementTrigger] ОТКАЗ:</color> Не все отверстия заполнены ящиками.");
        }
    }

    private bool CheckAllSlots()
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("[BoxPlacementTrigger] Массив Slots пуст! Перетащите отверстия в инспекторе рычага.");
            return false;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogError($"[BoxPlacementTrigger] В слоте №{i} пустая ссылка (Missing/Null)!");
                return false;
            }

            if (!slots[i].IsOccupied)
            {
                Debug.Log($"[BoxPlacementTrigger] Отверстие {slots[i].gameObject.name} на данный момент ПУСТОЕ.");
                return false;
            }
        }
        return true;
    }
}