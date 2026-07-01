using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    // Вспомогательный класс для настройки связи "Труба -> Нужный Угол"
    [System.Serializable]
    public struct NodeConnectionCondition
    {
        public PuzzlePipeNode node;
        [Tooltip("Угол по Y, при котором этот элемент считается соединенным для данного этапа")]
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

        bool allCorrect = true;
        foreach (var condition in finalConditions)
        {
            if (condition.node == null || !condition.node.IsAtAngle(condition.requiredAngle))
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            isFinalSolved = true;
            Debug.Log("<color=green>Головоломка полностью решена!</color>");
            onFinalPuzzleSolved?.Invoke();
            DisablePuzzleInteraction();
        }
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