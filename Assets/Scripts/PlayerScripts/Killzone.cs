using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       
        CheckpointMovement playerMovement = other.GetComponent<CheckpointMovement>();

        if (playerMovement != null)
        {
            playerMovement.RespawnAtLastSafePoint();
        }
    }
}