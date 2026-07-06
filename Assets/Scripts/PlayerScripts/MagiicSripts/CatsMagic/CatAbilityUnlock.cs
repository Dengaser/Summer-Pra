using UnityEngine;

public class CatAbilityUnlock : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private bool isCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag(playerTag))
        {
            // Ищем скрипт магий напрямую на игроке
            SwitchMagic playerMagic = other.GetComponent<SwitchMagic>();

            if (playerMagic != null)
            {
                isCollected = true;

                // Вызываем метод открытия, который мы только что добавили в SwitchMagic
                playerMagic.UnlockNextAbility();

                // Уничтожаем кота на сцене
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("На объекте игрока не найден скрипт SwitchMagic!");
            }
        }
    }
}