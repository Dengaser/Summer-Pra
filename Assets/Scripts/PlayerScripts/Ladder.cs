using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Передаем true, направление и сам этот коллайдер (this.GetComponent<Collider>())
                player.ToggleClimbing(true, transform.forward, GetComponent<Collider>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ToggleClimbing(false, Vector3.zero);
            }
        }
    }
}