using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Telekinesis : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip pickup;
    public AudioClip drop;
    public AudioClip push;

    [Header("Настройки дистанции")]
    public float distance = 5f;
    public float pushDistance = 8f;
   

    [Header("Ссылка на игрока")]
    public Transform playerTransform; 

    [Header("Интерфейс")]
    public Image cursorImage;
    public TextMeshProUGUI hintText;
    public Color baseColor = Color.white;
    public Color highlightColor = Color.green; 

    [Header("Точки удержания")]
    public Transform itemHoldParent;
    public Camera playerCamera;

    [Header("Настройки телекинеза")]
    public float throwForce = 15f;
    public float pushForce = 25f;

    [Header("Настройки луча от игрока")]
    public float eyeHeight = 0.5f;
    private GameObject _heldObject;
    public float boxWidth = 1f;
    public float boxHeight = 2f;

    private Vector3 BoxHalfExtents => new Vector3(boxWidth / 2f, boxHeight / 2f, 0.1f);
    protected virtual void Update()
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_heldObject == null) TryPickUp();
            else DropObject();
        }

        if (Input.GetKeyDown(KeyCode.R) && _heldObject == null)
        {
            TryPush();
        }
    }

    private void TryPickUp()
    {
        if (playerTransform == null) return;

        RaycastHit hit;
      
        if (CastBox(distance,out hit))
        {
            if (hit.collider.transform != playerTransform && hit.collider.CompareTag("Moveable"))
            {
                PickUpObject(hit.collider.gameObject);
            }
        }
    }

    private void TryPush()
    {
        if(playerTransform == null) return;
        RaycastHit hit;
        if(CastBox(pushDistance,out hit))
        {
            if(hit.collider.transform != playerTransform && hit.collider.CompareTag("Moveable") || hit.collider.CompareTag("Pushable"))
            {
                PushObject(hit.collider.gameObject);
            }
        }
    }

    private void PushObject(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 pushDirection = playerTransform != null ? playerTransform.forward : playerCamera.transform.forward;
            pushDirection.y += 0.1f;
            pushDirection.Normalize();

            rb.AddForce(pushDirection*pushForce, ForceMode.Impulse);

            if(AudioSource != null) AudioSource.PlayOneShot(push);


        }
    }
    private void PickUpObject(GameObject obj)
    {
        _heldObject = obj;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        obj.transform.SetParent(itemHoldParent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        if (AudioSource != null && pickup != null) AudioSource.PlayOneShot(pickup);
    }

    private void DropObject()
    {
        if (_heldObject == null) return;

        Rigidbody rb = _heldObject.GetComponent<Rigidbody>();

        _heldObject.transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

           
            Vector3 throwDirection = playerTransform != null ? playerTransform.forward : playerCamera.transform.forward;

           
            throwDirection.y += 0.1f;
            throwDirection.Normalize();

            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        _heldObject = null;
        if (AudioSource != null && drop != null) AudioSource.PlayOneShot(drop);
    }

    protected virtual bool CastBox(float customDistance, out RaycastHit hit)
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
        // Безопасная проверка на null
        if (hintText == null || playerTransform == null) return;

        // 1. Если мы уже держим объект — показываем подсказку телекинеза
        if (_heldObject != null)
        {
            if (cursorImage != null) cursorImage.color = highlightColor;
            hintText.text = "F: Положить объект";
            hintText.gameObject.SetActive(true);
            return;
        }

        RaycastHit hit;
        // 2. Стреляем нашей коробкой вперед
        if (CastBox(pushDistance, out hit))
        {
            // Проверяем, что это не сам игрок
            if (hit.collider.transform != playerTransform)
            {
                // --- ПРОВЕРКА НА ЗОМБИ ---
                // Проверяем тег Enemy или наличие компонента здоровья
                bool isZombie = hit.collider.CompareTag("Enemy") || hit.collider.GetComponent<HealthUniversal>() != null;

                if (isZombie)
                {
                    if (cursorImage != null) cursorImage.color = highlightColor;

                    // Берем кнопку атаки из наследника, если нужно, 
                    // но так как мы в базовом классе, напишем просто ЛКМ (или подставь свою)
                    hintText.text = "ЛКМ: Ударить зомби";
                    hintText.gameObject.SetActive(true);
                    return; // Успешно нашли зомби, выходим
                }


                // --- ПРОВЕРКА НА ПРЕДМЕТЫ ТЕЛЕКИНЕЗА ---
                bool isMoveable = hit.collider.CompareTag("Moveable");
                bool isPushable = hit.collider.CompareTag("Pushable");

                if (isMoveable || isPushable)
                {
                    if (cursorImage != null) cursorImage.color = highlightColor;

                    if (isMoveable && hit.distance <= distance)
                    {
                        hintText.text = "F: Взять | R: Толкнуть";
                    }
                    else
                    {
                        hintText.text = "R: Толкнуть";
                    }

                    hintText.gameObject.SetActive(true);
                    return; // Успешно нашли предмет, выходим
                }
            }
        }

        // 3. Если перед нами пустота или стена без нужных тегов — выключаем текст
        if (cursorImage != null) cursorImage.color = baseColor;
        hintText.gameObject.SetActive(false);
    }

    private void SetUI(Color c, string t)
    {
        cursorImage.color = c;
        hintText.text = t;
        hintText.gameObject.SetActive(true);
    }


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