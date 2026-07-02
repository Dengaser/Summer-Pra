using UnityEngine;

public class SlotZone : MonoBehaviour
{
    [SerializeField] private string targetTag = "Box"; // Любой объект с этим тегом подойдет
    private bool isOccupied = false;

    public bool IsOccupied => isOccupied;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isOccupied = false;
        }
    }
}