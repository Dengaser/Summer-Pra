using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class IceMagic : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip iceSound;

    [Header("Настройки дистанции")]
    public float distance = 5f;
    
    [Header("Ссылка на игрока")]
    public Transform playerTransform;
    public Camera playerCamera;


    [Header("Настройки дорожки льда")]
    public int iceBlocksCount = 5; // Сколько блоков льда будет в линии
    public float stepDistance = 1f; // Расстояние между блоками (равно размеру кубика льда)

    [Header("Интерфейс")]
    public TextMeshProUGUI hintText;
    [Header("Настройки луча от игрока")]
    public float eyeHeight = 0.5f; // Высота, откуда выходит луч (чуть выше пояса)[cite: 1]
    [Range(0f, 85f)]
    public float dipAngle = 35f;
    public ParticleSystem iceParticle;
    public float boxWidth = 1f;
    public float boxHeight = 2f;


    private Vector3 BoxHalfExtents => new Vector3(boxWidth / 2f, boxHeight / 2f, 0.1f);




    void Start()
    {
        
    }

    
    void Update()
    {
        if (playerCamera == null) playerCamera = Camera.main;

        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        UpdateUI();

        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            CastIce();
        }
    }

    private void CastIce()
    {
        if (playerTransform == null) return;

        // Переменные для хранения результатов попадания
        bool hitWater = CastRayDown(distance, out RaycastHit hit);
        bool hitFire = CastBox(distance, out RaycastHit hit_two);

        // Если никуда не попали — выходим
        if (!hitWater && !hitFire)
            return;

        // Эффекты проигрываем, если магия хоть куда-то попала
        if (iceParticle != null)
        {
            iceParticle.transform.forward = playerTransform.forward;
            iceParticle.Play();
        }

        if (AudioSource != null && iceSound != null)
            AudioSource.PlayOneShot(iceSound);

        // 1. ПРОВЕРКА ВОДЫ (Луч под ноги)
        if (hitWater && hit.collider.TryGetComponent(out Water water))
        {
            Vector3 forwardXZ = playerTransform.forward;
            forwardXZ.y = 0;
            forwardXZ.Normalize();

            Vector3 startPoint = hit.point;

            for (int i = 0; i < iceBlocksCount; i++)
            {
                Vector3 currentPoint = startPoint + (forwardXZ * (i * stepDistance));
                water.FreezeAtPoint(currentPoint);
            }
        }

        // 2. ПРОВЕРКА ОГНЯ (Коробка перед собой)
        // Используем 'if', а не 'else if', чтобы магия могла одновременно тушить огонь и морозить воду, если они рядом
        if (hitFire && hit_two.collider.transform != playerTransform && hit_two.collider.TryGetComponent(out IIceInteractable target))
        {
            target.OnFreeze();
        }
    }



    private bool CastRayDown(float customDistance, out RaycastHit hit)
    {
        // Берем направление "вперед" игрока и убираем Y, чтобы наклон головы не влиял
        Vector3 forwardXZ = playerTransform.forward;
        forwardXZ.y = 0;
        forwardXZ.Normalize();

        // Считаем стартовую точку (на уровне "глаз" / груди игрока)
        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight;

        // Наклоняем вектор вперед вниз на заданный угол (dipAngle)
        Vector3 rayDirection = Quaternion.AngleAxis(dipAngle, playerTransform.right) * forwardXZ;

        // Пускаем обычный Raycast, который идеально распознает MeshCollider воды
        return Physics.Raycast(rayStartPoint, rayDirection, out hit, customDistance);
    }

    private bool CastBox(float customDistance, out RaycastHit hit)
    {
        Vector3 rayDirection = playerTransform.forward;
        rayDirection.y = 0;
        rayDirection.Normalize();
        float spawnOffset = 0.6f;
        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight + rayDirection * spawnOffset;
        float castDistance = Mathf.Max(0.1f, customDistance - spawnOffset);

        return Physics.BoxCast(rayStartPoint, BoxHalfExtents, rayDirection, out hit, playerTransform.rotation, castDistance);

    }

    private void UpdateUI()
    {
        if (hintText == null || playerTransform == null) //[cite: 1]
            return; //[cite: 1]

        if (CastRayDown(distance, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Water")) //[cite: 1]
            {
                hintText.text = "R|ЛКМ: Заморозить воду "; //[cite: 1]
                hintText.gameObject.SetActive(true); //[cite: 1]
                return; //[cite: 1]
            }
        }
        if (CastBox(distance, out RaycastHit hit_two))
        {
            if (hit_two.collider.CompareTag("Fire")) 
            {
                hintText.text = "R|ЛКМ: Потушить огонь"; 
                hintText.gameObject.SetActive(true); 
                return; 
            }
        }
            
        

        hintText.gameObject.SetActive(false); //[cite: 1]
    }


    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;

        Vector3 forwardXZ = playerTransform.forward;
        forwardXZ.y = 0;
        forwardXZ.Normalize();

        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight;
        Vector3 rayDirection = Quaternion.AngleAxis(dipAngle, playerTransform.right) * forwardXZ;
        

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(rayStartPoint, rayDirection * distance);


        // Определяем точку старта луча
        
        Vector3 rayDirection_two = playerTransform.forward;
        rayDirection_two.y = 0;
        rayDirection_two.Normalize();

        // Центр коробки находится на половине дистанции луча впереди игрока
        Vector3 boxCenter = rayStartPoint + rayDirection_two * (distance / 2f);

        // Устанавливаем матрицу Gizmos, чтобы коробка крутилась вслед за персонажем
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, playerTransform.rotation, Vector3.one);

        // Рисуем объемную коробку захвата (размеры: ширина, высота, длина луча)
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.3f); // Полупрозрачный желтый
        Gizmos.DrawCube(Vector3.zero, new Vector3(boxWidth, boxHeight, distance));

        Gizmos.color = Color.yellow; // Контур коробки
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(boxWidth, boxHeight, distance));

    }
}
