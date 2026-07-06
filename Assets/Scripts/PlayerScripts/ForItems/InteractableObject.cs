using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum ObjectType
    {
        Door,
        Button,
        Board,
        PuzzleNode,
        PuzzleNode2,
            Lever,
            BoxPuzzleTrigger,
        ObjectResetter,
            CauldronSpawner// <-- ДОБАВИЛИ: Элемент головоломки-цепи
    }

    [Header("Выберите тип этого предмета:")]
    public ObjectType typeOfObject;

    // Ссылка на компонент вращения (заполняется автоматически, если это PuzzleNode)
    private PuzzlePipeNode pipeNode;
    private PuzzlePipeNode2 pipeNode2;

    [Header("Настройки для возврата объекта (если выбран ObjectResetter):")]
    [SerializeField] private ReturnableObject targetReturnableObject;

    private void Awake()
    {
        if (typeOfObject == ObjectType.PuzzleNode)
        {
            pipeNode = GetComponent<PuzzlePipeNode>();
            pipeNode2 = GetComponent<PuzzlePipeNode2>();
        }
    }


    // --- ЛОГИКА ДЛЯ ДОСКИ ---
    [Header("Настройки для доски (если выбрана Board):")]
    private bool moved = false;
    private Vector3 targetLocalPosition = new Vector3(-51.3844757f, 56.6100006f, -28.0100002f);
    private Quaternion targetLocalRotation = Quaternion.Euler(-105.062f, -0.8009949f, 178.578f);

    public void OnInteract()
    {
        switch (typeOfObject)
        {
            case ObjectType.Door:
                LogicForDoor();
                break;
            case ObjectType.Button:
                LogicForButton();
                break;
            case ObjectType.Board:
                LogicForBoard();
                break;
            case ObjectType.PuzzleNode: // <-- ДОБАВИЛИ: Вызов логики поворота трубы/цепи
                LogicForPuzzleNode();
                break;

            case ObjectType.Lever:
                LogicForLever();
                break;
            case ObjectType.BoxPuzzleTrigger: // <-- ДОБАВИЛИ: Вызов логики проверки ящиков
                LogicForBoxPuzzleTrigger();
                break;

            case ObjectType.ObjectResetter: // <-- ДОБАВИЛИ: Вызов новой логики
                LogicForResetObject();
                break;
            case ObjectType.CauldronSpawner: // <-- ДОБАВИЛИ: Вызов логики спавна
                LogicForCauldronSpawner();
                break;
        }
    }

    private void LogicForDoor() => Debug.Log("Логика открытия двери");
    private void LogicForButton() => Debug.Log("Логика нажатия кнопки");

    private void LogicForBoard()
    {
        if (moved) return;
        moved = true;
        transform.localPosition = targetLocalPosition;
        transform.localRotation = targetLocalRotation;
        Debug.Log("Доска успешно перемещена!");
    }

    // <-- ДОБАВИЛИ: Метод вращения элемента цепи
    private void LogicForPuzzleNode()
    {
        // 1. Сначала проверяем, есть ли старый скрипт
        if (pipeNode != null)
        {
            pipeNode.RotateNode();
        }
        // 2. Если старого нет, проверяем, есть ли новый
        else if (pipeNode2 != null)
        {
            pipeNode2.RotateNode();
        }
        // 3. Если вообще ничего не найдено
        else
        {
            Debug.LogError($"На объекте {gameObject.name} выбран тип PuzzleNode, но ни PuzzlePipeNode, ни PuzzlePipeNode2 не найдены!");
        }
    }
    private void LogicForLever()
    {
        // Ищем скрипт рычага на этом же объекте и активируем его
        LeverController lever = GetComponent<LeverController>();
        if (lever != null)
        {
            lever.PullLever();
        }
    }
    private void LogicForBoxPuzzleTrigger()
    {
        BoxPlacementTrigger trigger = GetComponent<BoxPlacementTrigger>();
        if (trigger != null)
        {
            trigger.TryActivate();
        }
        else
        {
            Debug.LogError($"На объекте {gameObject.name} выбран тип BoxPuzzleTrigger, но скрипт BoxPlacementTrigger не найден!");
        }
    }
    private void LogicForResetObject()
    {
        if (targetReturnableObject != null)
        {
            targetReturnableObject.ResetToTargetPosition();
        }
        else
        {
            Debug.LogError($"На объекте {gameObject.name} выбран тип ObjectResetter, но не указана ссылка на Target Returnable Object в инспекторе!");
        }
    }

    private void LogicForCauldronSpawner()
    {
        CauldronItemSpawner spawner = GetComponent<CauldronItemSpawner>();
        if (spawner != null)
        {
            spawner.SpawnItem();
        }
        else
        {
            Debug.LogError($"На объекте {gameObject.name} выбран тип CauldronSpawner, но скрипт CauldronItemSpawner не найден!");
        }
    }
}