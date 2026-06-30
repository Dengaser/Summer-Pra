using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerViewHelper : MonoBehaviour
{
    public float distance = 5f; //дистанция взгляда

    public Transform playerTransform; //ссылка на игрока
    public Camera playerCamera; //ссылка на камеру

    [Header("Настройки луча от игрока")]
    public float eyeHeight = 1f; //высота взгляда, середина квадрата
    private GameObject _currentObject; //объект с которым нужно взаимодействовать
    public float boxWidth = 1f; //ширина взгляда
    public float boxHeight = 2f;//высота взгляда, настроить под рост игрока


    [Header("Интерфейс")]
    public TextMeshProUGUI hintText;
   

    private Vector3 BoxHalfExtents => new Vector3(boxWidth / 2f, boxHeight / 2f, 0.1f);
   

    
    void Update()
    {
        if (playerCamera == null) playerCamera = Camera.main;

        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        HandleInput();
        UpdateUI();

    }



    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryUsing();
        }

    }


    private void TryUsing()
    {
        if (playerTransform == null) return;
        RaycastHit hit;
        if(Cast(distance,out hit))
        {
            if(hit.collider.transform != playerTransform && hit.collider.CompareTag("Useable")) //тег Useable необходимо дать предметам
            {
                InteractableObject target = hit.collider.GetComponent<InteractableObject>();
                if (target != null)
                {
                    target.OnInteract(); // Вызываешь логику, которую написал для предмета
                }
            }
        }
    }


   




    private void UpdateUI()
    {
        if (hintText == null || playerTransform == null) return;

        RaycastHit hit;
        if (Cast(distance, out hit) && hit.collider.transform != playerTransform)
        {
            if (hit.collider.CompareTag("Useable")) //тег Useable необходимо дать предметам
            {
                SetUI("E: Использовать");
                return;
            }
        }
        hintText.gameObject.SetActive(false);
    }

    private void SetUI(string t)
    {
        hintText.text = t;
        hintText.gameObject.SetActive(true);
    }




    //метод для взгляда-коробки
    private bool Cast(float customDistance, out RaycastHit hit)
    {
        Vector3 rayDirection = playerTransform.forward;
        rayDirection.y = 0;
        rayDirection.Normalize();
        float spawnOffset = 0.6f;
        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight + rayDirection * spawnOffset;
        float castDistance = Mathf.Max(0.1f, customDistance - spawnOffset);

        return Physics.BoxCast(rayStartPoint, BoxHalfExtents, rayDirection, out hit, playerTransform.rotation, castDistance);
    }

    //метод для проверки вгляда в сцене
    private void OnDrawGizmos()
    {
        if (playerTransform == null) return;

        // Определяем точку старта луча
        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight;
        Vector3 rayDirection = playerTransform.forward;
        rayDirection.y = 0;
        rayDirection.Normalize();

        // Центр коробки находится на половине дистанции луча впереди игрока
        Vector3 boxCenter = rayStartPoint + rayDirection * (distance / 2f);

        // Устанавливаем матрицу Gizmos, чтобы коробка крутилась вслед за персонажем
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, playerTransform.rotation, Vector3.one);

        // Рисуем объемную коробку захвата (размеры: ширина, высота, длина луча)
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.3f); // Полупрозрачный желтый
        Gizmos.DrawCube(Vector3.zero, new Vector3(boxWidth, boxHeight, distance));

        Gizmos.color = Color.yellow; // Контур коробки
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(boxWidth, boxHeight, distance));
    }
}
