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
        if (playerTransform == null) return; //

        // Пускаем наклонный луч, чтобы найти ПЕРВУЮ точку на воде (под ногами или чуть впереди)
        if (!CastRayDown(distance, out RaycastHit hit))
            return;

        if (iceParticle != null) 
        {
            iceParticle.transform.forward = playerTransform.forward; 
            iceParticle.Play(); 
        }

        if (AudioSource != null && iceSound != null) 
            AudioSource.PlayOneShot(iceSound); 

        // Проверяем, что попали именно в воду
        if (hit.collider.TryGetComponent(out Water water))
        {
            // Получаем направление взгляда игрока на плоскости XZ (чтобы линия не уходила вверх или вниз)
            Vector3 forwardXZ = playerTransform.forward;
            forwardXZ.y = 0;
            forwardXZ.Normalize();

            // Точка первого попадания луча
            Vector3 startPoint = hit.point;

            // Цикл для создания линии из кубиков
            for (int i = 0; i < iceBlocksCount; i++)
            {
                // Вычисляем позицию для каждого следующего кубика вдоль направления взгляда
                Vector3 currentPoint = startPoint + (forwardXZ * (i * stepDistance));

                // Вызываем заморозку в этой точке
                water.FreezeAtPoint(currentPoint);
            }
        }
        // Если попали во что-то другое с интерфейсом льда (например, огонь)
        else if (hit.collider.transform != playerTransform && hit.collider.TryGetComponent(out IIceInteractable target))
        {
            target.OnFreeze(); //[cite: 1]
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

            if (hit.collider.CompareTag("Fire")) //[cite: 1]
            {
                hintText.text = "R|ЛКМ: Потушить огонь"; //[cite: 1]
                hintText.gameObject.SetActive(true); //[cite: 1]
                return; //[cite: 1]
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
    }
}
