using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FireMagic : MonoBehaviour
{
    private Animator animator;
    public AudioSource AudioSource;
    public AudioClip fireSound;

    [Header("Настройки дистанции")]
    public float distance = 5f;

    [Header("Ссылка на игрока")]
    public Transform playerTransform;
    public Camera playerCamera;


   

    [Header("Интерфейс")]
    public TextMeshProUGUI hintText;
    [Header("Настройки луча от игрока")]
    public float eyeHeight = 0.5f;
    public float boxWidth = 1f;
    public float boxHeight = 2f;
    public ParticleSystem fireParticle;

    private Vector3 BoxHalfExtents => new Vector3(boxWidth / 2f, boxHeight / 2f, 0.1f);

    void Start()
    {
        animator = GetComponent<Animator>();

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
            animator.SetTrigger("Skill");
            CastFire();
        }
    }



    private RaycastHit[] CastBoxAll(float customDistance)
    {
        Vector3 rayDirection = playerTransform.forward;
        rayDirection.y = 0;
        rayDirection.Normalize();
        float spawnOffset = 0.6f;
        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight + rayDirection * spawnOffset;
        float castDistance = Mathf.Max(0.1f, customDistance - spawnOffset);

        // BoxCastAll находит ВСЕ цели по траектории коробки
        return Physics.BoxCastAll(rayStartPoint, BoxHalfExtents, rayDirection, playerTransform.rotation, castDistance);
    }

    private void CastFire()
    {
        if (playerTransform == null) return;

        // Получаем все цели в зоне поражения
        RaycastHit[] hits = CastBoxAll(distance);

        if (hits == null || hits.Length == 0)
            return;

        // Эффекты запускаем один раз при касте
        if (fireParticle != null)
        {
            fireParticle.transform.forward = playerTransform.forward;
            fireParticle.Play();
        }

        if (AudioSource != null && fireSound != null)
            AudioSource.PlayOneShot(fireSound);

        // Проходим циклом по ВСЕМ попаданиям
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform != playerTransform && hit.collider.TryGetComponent(out IFireInteractable target))
            {
                target.OnFire();
            }
        }
    }

    private void UpdateUI()
    {
        if (hintText == null || playerTransform == null)
            return;

        RaycastHit[] hits = CastBoxAll(distance);

        // Проверяем, есть ли среди целей хоть один зомби или поджигаемый объект
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hintText.text = "R|ЛКМ: Поджечь зомби";
                hintText.gameObject.SetActive(true);
                return; // Выходим из метода, так как текст уже включили
            }
            if (hit.collider.CompareTag("Burn"))
            {
                hintText.text = "R|ЛКМ: Поджечь";
                hintText.gameObject.SetActive(true);
                return;
            }
        }

        hintText.gameObject.SetActive(false);
    }


    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;

        Vector3 forwardXZ = playerTransform.forward;
        forwardXZ.y = 0;
        forwardXZ.Normalize();

        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight;
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
