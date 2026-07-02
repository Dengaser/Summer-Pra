using System.Collections;
using UnityEngine;

public class ChapterEndController : MonoBehaviour
{
    [Header("Перетащите сюда главный Canvas (на котором висит Canvas Group)")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 2.5f; // Время потемнения экрана в секундах

    private void Awake()
    {
        // При старте игры принудительно делаем интерфейс невидимым и прозрачным для кликов
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void TriggerChapterEnd()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("[ChapterEndController] Ссылка на CanvasGroup не указана!");
            return;
        }

        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        Debug.Log("[ChapterEndController] Корутина плавного угасания экрана запущена успешно.");
        canvasGroup.blocksRaycasts = true;
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        Debug.Log("[ChapterEndController] Экран полностью стал черным, текст отображен.");
    }
}