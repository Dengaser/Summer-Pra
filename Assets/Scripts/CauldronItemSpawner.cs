using UnityEngine;

public class CauldronItemSpawner : MonoBehaviour
{
    [Header("Префаб предмета для этого ящика")]
    [SerializeField] private GameObject itemPrefab;

    [Header("Точка появления предмета")]
    [SerializeField] private Transform spawnPoint;

    // Статическая переменная хранит ОДИН текущий активный предмет на всей сцене
    private static GameObject currentActiveItem;

    public void SpawnItem()
    {
        if (itemPrefab == null || spawnPoint == null)
        {
            Debug.LogError($"Настройки спавнера на {gameObject.name} не заполнены!");
            return;
        }

        // Если на сцене уже лежит какой-то предмет, взятый из ЛЮБОГО такого ящика — уничтожаем его
        if (currentActiveItem != null)
        {
            Destroy(currentActiveItem);
            Debug.Log("Предыдущий предмет уничтожен, так как взят новый.");
        }

        // Создаем новый предмет
        currentActiveItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);

        // Отрезаем (Clone), чтобы имя объекта четко соответствовало префабу для котла
        currentActiveItem.name = itemPrefab.name;

        Debug.Log($"Успешно заспавнен предмет: {currentActiveItem.name}");
    }
}