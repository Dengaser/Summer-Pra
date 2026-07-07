using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Trigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level3");
        }
    }
}

