using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum ObjectType
    {
        Door,
        Button,
        Board,
        PuzzleNode // <-- ДОБАВИЛИ: Элемент головоломки-цепи
    }

    [Header("Выберите тип этого предмета:")]
    public ObjectType typeOfObject;

    // Ссылка на компонент вращения (заполняется автоматически, если это PuzzleNode)
    private PuzzlePipeNode pipeNode;

    private void Awake()
    {
        if (typeOfObject == ObjectType.PuzzleNode)
        {
            pipeNode = GetComponent<PuzzlePipeNode>();
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
        if (pipeNode != null)
        {
            pipeNode.RotateNode();
        }
    }
}