using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Префаб предмета, который лежит в этом ящике")]
    [SerializeField] private GameObject itemPrefab;

    [Header("Точка, где будет появляться предмет (например, чуть выше ящика)")]
    [SerializeField] private Transform spawnPoint;

    // Статическая переменная, общая для ВСЕХ ящиков. 
    // Она будет хранить ссылку на ОДИН единственный активный предмет в мире.
    private static GameObject currentActiveItem;

    private bool isPlayerNearby = false;

    void Update()
    {
        // Если игрок рядом и нажал кнопку взаимодействия (например, E)
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        // 1. Если в мире УЖЕ есть какой-то заспавненный предмет — уничтожаем его
        if (currentActiveItem != null)
        {
            Destroy(currentActiveItem);
            Debug.Log("Предыдущий предмет уничтожен, так как взят новый.");
        }

        // 2. Создаем новый предмет в точке спавна
        currentActiveItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);

        // Переименовываем обратно, убирая "(Clone)", чтобы котел узнал имя предмета!
        currentActiveItem.name = itemPrefab.name;

        Debug.Log($"Заспавнен предмет: {currentActiveItem.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}