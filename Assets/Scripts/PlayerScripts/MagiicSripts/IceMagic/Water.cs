using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class Water : MonoBehaviour, IIceInteractable
{
    [Header("Настройки заморозки")]
    [SerializeField] private GameObject icePrefab; 
    [SerializeField] private float iceBlockSize = 1f; // Размер блока льда для выравнивания по сетке (0, если сетка не нужна)

    
    private HashSet<Vector3> frozenPositions = new HashSet<Vector3>();

    
    public void OnFreeze()
    {
       
    }

    
    public void FreezeAtPoint(Vector3 hitPoint)
    {
        if (icePrefab == null) return;

       
        Vector3 spawnPosition = hitPoint;
        if (iceBlockSize > 0)
        {
            spawnPosition.x = Mathf.Round(hitPoint.x / iceBlockSize) * iceBlockSize;
           
            spawnPosition.y = transform.position.y;
            spawnPosition.z = Mathf.Round(hitPoint.z / iceBlockSize) * iceBlockSize;
        }

   
        if (!frozenPositions.Contains(spawnPosition))
        {
          
            GameObject newIce = Instantiate(icePrefab, spawnPosition, Quaternion.identity);

            
            newIce.transform.SetParent(transform);

           
            frozenPositions.Add(spawnPosition);
        }
    }
}
   
