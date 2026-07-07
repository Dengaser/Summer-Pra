using UnityEngine;

public class CauldronCore : MonoBehaviour
{
    [Header("Правильная последовательность имен объектов")]
    [SerializeField] private string[] correctSequence = { "Apple", "Mushroom", "Key", "Potion" };

    [Header("UI Элемент победы")]
    [SerializeField] private GameObject winTextObject;

    [Header("Эффекты")]
    [SerializeField] private ParticleSystem errorParticles;   // Эффект при ошибке
    [SerializeField] private ParticleSystem successParticles; // <-- ДОБАВИЛИ: Эффект при ПРАВИЛЬНОМ предмете

    private int currentStep = 0;

    void Start()
    {
        if (winTextObject != null)
        {
            winTextObject.SetActive(false);
        }

        // Выключаем эффекты на старте
        if (errorParticles != null) errorParticles.Stop();
        if (successParticles != null) successParticles.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, совпадает ли имя упавшего объекта с текущим шагом
        if (other.gameObject.name == correctSequence[currentStep])
        {
            Debug.Log($"Правильный предмет: {other.gameObject.name}!");
            currentStep++;

            // Включаем эффект успеха
            PlayEffect(successParticles);

            // Уничтожаем предмет
            Destroy(other.gameObject);

            // Проверяем победу
            if (currentStep >= correctSequence.Length)
            {
                WinGame();
            }
        }
        else
        {
            // Если в котел упал не тот предмет (но при этом у него есть коллайдер)
            if (other.CompareTag("QuestItem") || other.gameObject.GetComponent<Rigidbody>() != null)
            {
                Debug.Log($"Неверный порядок! Ожидалось: {correctSequence[currentStep]}, а бросили: {other.gameObject.name}. Сброс!");

                // Включаем эффект ошибки
                PlayEffect(errorParticles);

                ResetCauldron();
                Destroy(other.gameObject); // Уничтожаем ошибочный предмет
            }
        }
    }

    private void WinGame()
    {
        if (winTextObject != null)
        {
            winTextObject.SetActive(true);
        }
        Debug.Log("Вы спасли мир!");
    }

    private void ResetCauldron()
    {
        currentStep = 0;
    }

    // Универсальный метод для запуска частиц
    private void PlayEffect(ParticleSystem particles)
    {
        if (particles != null)
        {
            particles.Stop();
            particles.Play();
        }
    }
}