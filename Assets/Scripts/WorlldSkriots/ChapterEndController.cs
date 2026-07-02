using System.Collections;
using UnityEngine;

public class ChapterEndController : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [SerializeField] private CanvasGroup canvasGroup; // Сюда перетащим наш Canvas

    [Header("Настройки")]
    [SerializeField] private float fadeDuration = 2.5f; // За сколько секунд экран полностью потемнеет

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    /// <summary>
    /// Метод для запуска финала главы
    /// </summary>
    public void TriggerChapterEnd()
    {
        if (canvasGroup == null) return;

        StopAllCoroutines();
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        Debug.Log("Финал главы: Экран начинает темнеть через UI и Код...");

        // Блокируем клики мыши, чтобы игрок больше не мог взаимодействовать с миром
        canvasGroup.blocksRaycasts = true;

        float currentTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            // Плавно увеличиваем прозрачность от текущей до 1
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, currentTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}