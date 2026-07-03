using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class Water : MonoBehaviour, IIceInteractable
{
    [Header("Настройки заморозки")]
    [SerializeField] private GameObject icePrefab; // Префаб кусочка льда
    [SerializeField] private float iceBlockSize = 1f; // Размер блока льда для выравнивания по сетке (0, если сетка не нужна)

    // Список уже созданных кусочков льда, чтобы не спавнить их в одном и том же месте
    private HashSet<Vector3> frozenPositions = new HashSet<Vector3>();

    // Обычный метод интерфейса (на случай вызова без параметров)
    public void OnFreeze()
    {
        // Можно оставить пустым или вызывать базовую логику
    }

    // Кастомный метод, который мы вызовем из IceMagic, передавая точку попадания
    public void FreezeAtPoint(Vector3 hitPoint)
    {
        if (icePrefab == null) return;

        // Определяем позицию спавна. 
        // Если iceBlockSize > 0, округляем координаты, чтобы лед вставал ровными кубиками (сеткой)
        Vector3 spawnPosition = hitPoint;
        if (iceBlockSize > 0)
        {
            spawnPosition.x = Mathf.Round(hitPoint.x / iceBlockSize) * iceBlockSize;
            // Высоту (Y) обычно лучше брать фиксированной по поверхности воды
            spawnPosition.y = transform.position.y;
            spawnPosition.z = Mathf.Round(hitPoint.z / iceBlockSize) * iceBlockSize;
        }

        // Проверяем, нет ли в этой точке уже льда
        if (!frozenPositions.Contains(spawnPosition))
        {
            // Спавним лед
            GameObject newIce = Instantiate(icePrefab, spawnPosition, Quaternion.identity);

            // Родителем делаем объект воды, чтобы не засорять иерархию
            newIce.transform.SetParent(transform);

            // Запоминаем позицию
            frozenPositions.Add(spawnPosition);
        }
    }
}
   
