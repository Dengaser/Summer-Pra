using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в пропасть упал именно игрок
        CheckpointMovement playerMovement = other.GetComponent<CheckpointMovement>();

        if (playerMovement != null)
        {
            playerMovement.RespawnAtLastSafePoint();
        }
    }
}