using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // Создаем список типов объектов
    public enum ObjectType
    {
        Door,
        Button //сюда пишешь тип предмета
    }

    [Header("Выберите тип этого предмета:")]
    public ObjectType typeOfObject;

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

           

             //сюда пишешь новый кейс под новый тип предмета и вызываешь логику
        }
    }

    // Ниже описываешь функции под каждый тип
    private void LogicForDoor()
    {
        Debug.Log("Логика открытия двери");
    }


    private void LogicForButton()
    {
        Debug.Log("Логика нажатия кнопки");
    }
}