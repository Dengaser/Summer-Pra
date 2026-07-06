using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager2 : MonoBehaviour
{
    [Header("Ссылка на платформу-замыкатель")]
    public PlatformController requiredPlatform;

    // Вспомогательный класс для настройки связи "Труба -> Нужный Угол"
    [System.Serializable]
    public struct NodeConnectionCondition
    {
        // МЕНЯЕМ ТИП ТУТ: теперь используется новый скрипт PuzzlePipeNode2
        public PuzzlePipeNode2 node;
        [Tooltip("Целевой угол (в градусах) для выбранной на элементе оси, при котором он считается соединенным")]
        public float requiredAngle;
    }

    [System.Serializable]
    public struct PuzzleMilestone
    {
        public string name; // Название (например, "Опускание моста")
        [Header("Условия для выполнения этого этапа:")]
        public NodeConnectionCondition[] conditions;
        public UnityEvent onMilestoneReached;
        [HideInInspector] public bool isReached;
    }

    [Header("Промежуточные этапы (Мост и т.д.)")]
    public PuzzleMilestone[] milestones;

    [Header("Финальный этап (Открытие двери)")]
    public NodeConnectionCondition[] finalConditions;
    public UnityEvent onFinalPuzzleSolved;

    private bool isFinalSolved = false;

    public void CheckPuzzleDelayed()
    {
        CancelInvoke(nameof(VerifyPuzzle));
        Invoke(nameof(VerifyPuzzle), 0.2f);
    }

    private void VerifyPuzzle()
    {
        CheckMilestones();
        CheckFinalSolution();
    }

    private void CheckMilestones()
    {
        for (int i = 0; i < milestones.Length; i++)
        {
            if (milestones[i].isReached) continue;

            bool milestoneCorrect = true;
            foreach (var condition in milestones[i].conditions)
            {
                if (condition.node == null || !condition.node.IsAtAngle(condition.requiredAngle))
                {
                    milestoneCorrect = false;
                    break;
                }
            }

            if (milestoneCorrect)
            {
                milestones[i].isReached = true;
                Debug.Log($"<color=orange>Этап '{milestones[i].name}' выполнен!</color>");
                milestones[i].onMilestoneReached?.Invoke();
            }
        }
    }

    private void CheckFinalSolution()
    {
        if (isFinalSolved || finalConditions == null || finalConditions.Length == 0) return;

        if (requiredPlatform != null && !requiredPlatform.IsPlatformDown())
        {
            Debug.LogWarning("Головоломка собрана, но Платформа НЕ опущена!");
            return;
        }

        bool allCorrect = true;
        foreach (var condition in finalConditions)
        {
            if (!IsConditionMet(condition))
            {
                if (condition.node != null)
                {
                    // Лог подскажет, какой именно объект менеджер считает неверным
                    Debug.Log($"Объект {condition.node.name} НЕ в правильном угле. Нужно: {condition.requiredAngle}");
                }
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            isFinalSolved = true;
            Debug.Log("<color=green>Головоломка полностью решена! Дверь открывается.</color>");
            onFinalPuzzleSolved?.Invoke();
            DisablePuzzleInteraction();
        }
    }

    // --- ВОТ ЭТОТ МЕТОД, КОТОРОГО НЕ ХВАТАЛО ---
    private bool IsConditionMet(NodeConnectionCondition condition)
    {
        if (condition.node == null) return false;

        // Так как condition.node уже является типом PuzzlePipeNode2, 
        // мы можем вызвать метод IsAtAngle напрямую!
        return condition.node.IsAtAngle(condition.requiredAngle);
    }

    private void DisablePuzzleInteraction()
    {
        foreach (var condition in finalConditions)
        {
            if (condition.node == null) continue;
            var interactable = condition.node.GetComponent<InteractableObject>();
            if (interactable != null) interactable.enabled = false;
        }
    }
}