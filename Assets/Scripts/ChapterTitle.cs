using System.Collections;
using UnityEngine;
using TMPro;

public class ChapterTitle : MonoBehaviour
{
    [Header("Настройки UI")]
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private string titleText = "Глава 1";

    [Header("Настройки анимации (в секундах)")]
    [SerializeField] private float fadeInDuration = 1.5f;  // Сколько времени текст проявляется
    [SerializeField] private float displayDuration = 2.0f; // Сколько секунд горит на полную
    [SerializeField] private float fadeOutDuration = 1.5f; // Сколько времени текст затухает

    private void Start()
    {
        if (chapterText != null)
            StartCoroutine(ShowTitleRoutine());
        else
            Debug.LogError("Критическая ошибка: Не назначен компонент TextMeshPro в инспекторе!");
    }

    private IEnumerator ShowTitleRoutine()
    {
        // Назначаем текст
        chapterText.text = titleText;

        // Берем текущий цвет текста и делаем его полностью прозрачным (альфа = 0)
        Color textColor = chapterText.color;
        textColor.a = 0f;
        chapterText.color = textColor;

        // Включаем объект
        chapterText.gameObject.SetActive(true);

        // 1. ПЛАВНОЕ ПОЯВЛЕНИЕ (Fade In)
        float currentTime = 0f;
        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            // Рассчитываем прозрачность от 0 до 1
            textColor.a = Mathf.Lerp(0f, 1f, currentTime / fadeInDuration);
            chapterText.color = textColor;
            yield return null; // Ждем один кадр перед следующим шагом
        }

        // На всякий случай жестко ставим 1 в конце цикла
        textColor.a = 1f;
        chapterText.color = textColor;

        // 2. ОЖИДАНИЕ
        yield return new WaitForSeconds(displayDuration);

        // 3. ПЛАВНОЕ ИСЧЕЗНОВЕНИЕ (Fade Out)
        currentTime = 0f;
        while (currentTime < fadeOutDuration)
        {
            currentTime += Time.deltaTime;
            // Рассчитываем прозрачность от 1 до 0
            textColor.a = Mathf.Lerp(1f, 0f, currentTime / fadeOutDuration);
            chapterText.color = textColor;
            yield return null;
        }

        // Жестко ставим 0 и выключаем объект
        textColor.a = 0f;
        chapterText.color = textColor;
        chapterText.gameObject.SetActive(false);
    }
}