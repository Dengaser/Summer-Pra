using UnityEngine;

public class Cauldron : MonoBehaviour
{
    [Header("Точные имена объектов в нужной последовательности")]
    [SerializeField] private string[] correctSequence = { "Apple", "Mushroom", "Key", "Potion" };

    [Header("UI Элементы")]
    [SerializeField] private GameObject winTextObject;

    private int currentStep = 0;

    void Start()
    {
        if (winTextObject != null)
        {
            winTextObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, совпадает ли ИМЯ объекта с тем, что мы ждем на этом шаге
        if (other.gameObject.name == correctSequence[currentStep])
        {
            Debug.Log($"Правильный предмет! {other.gameObject.name} засчитан.");
            currentStep++;

            Destroy(other.gameObject);

            if (currentStep >= correctSequence.Length)
            {
                WinGame();
            }
        }
        else
        {
            // Сюда код попадет, если бросили не тот предмет, ИЛИ тот, но не вовремя
            Debug.Log($"Отказ! Ожидалось: {correctSequence[currentStep]}, а бросили: {other.gameObject.name}");
            ResetCauldron();
        }
    }

    private void WinGame()
    {
        Debug.Log("Победа! Мир спасен!");
        if (winTextObject != null)
        {
            winTextObject.SetActive(true);
        }
    }

    private void ResetCauldron()
    {
        currentStep = 0;
        // Здесь можно проиграть звук шипения или заспавнить предметы заново
    }
}