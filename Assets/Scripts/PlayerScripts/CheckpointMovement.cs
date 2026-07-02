using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Обязательно добавляем для работы с компонентом Image

public class CheckpointMovement : MonoBehaviour
{
    [Header("Настройки перемещения")]
    [Tooltip("Как часто сохранять позицию (в секундах)")]
    [SerializeField] private float saveInterval = 0.5f;

    [Header("Настройки затемнения")]
    [Tooltip("Ссылка на черную картинку из Canvas")]
    [SerializeField] private Image fadeImage;
    [Tooltip("Длительность эффекта затемнения (в секундах)")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Ссылки")]
    [SerializeField] private CharacterController characterController;

    private Vector3 lastSafePosition;
    private Quaternion lastSafeRotation; // Теперь запоминаем и направление взгляда
    private bool isGrounded;
    private bool isRespawning = false; // Блокировка повторного падения во время респавна

    void Start()
    {
        lastSafePosition = transform.position;
        lastSafeRotation = transform.rotation;

        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        // Убедимся, что при старте экран не затемнен
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        StartCoroutine(SavePositionRoutine());
    }

    void Update()
    {
        if (characterController != null)
        {
            isGrounded = characterController.isGrounded;
        }
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            isGrounded = rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.01f;
        }
    }

    private IEnumerator SavePositionRoutine()
    {
        while (true)
        {
            // Сохраняем позицию ТОЛЬКО если персонаж на земле и НЕ находится в процессе респавна
            if (isGrounded && !isRespawning)
            {
                lastSafePosition = transform.position;
                lastSafeRotation = transform.rotation;
            }
            yield return new WaitForSeconds(saveInterval);
        }
    }

    // Этот метод теперь просто запускает корутину респавна
    public void RespawnAtLastSafePoint()
    {
        if (!isRespawning)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    // Процесс плавного респавна с затемнением
    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        // 1. Уходим в затемнение (Fade Out)
        yield return StartCoroutine(Fade(0f, 1f));

        // Небольшая пауза в полной темноте для дезориентации
        yield return new WaitForSeconds(0.1f);

        // 2. Перемещаем персонажа
        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = lastSafePosition;
            transform.rotation = lastSafeRotation;
            characterController.enabled = true;
        }
        else
        {
            transform.position = lastSafePosition;
            transform.rotation = lastSafeRotation;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Небольшая пауза после перемещения перед открытием экрана
        yield return new WaitForSeconds(0.2f);

        // 3. Выходим из затемнения (Fade In)
        yield return StartCoroutine(Fade(1f, 0f));

        isRespawning = false;
    }

    // Универсальный метод для плавного изменения прозрачности UI-картинки
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // Плавно интерполируем альфа-канал во времени
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Фиксируем финальное значение, чтобы избежать погрешностей Lerp
        color.a = endAlpha;
        fadeImage.color = color;
    }
}