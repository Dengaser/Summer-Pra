using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Trigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SwitchMagic playerMagic = other.GetComponent<SwitchMagic>();
            if (playerMagic != null)
            {
                playerMagic.SaveProgress(); 
                Debug.Log("[Level3Trigger] Прогресс котов сохранен перед переходом на Level3.");
            }
            SceneManager.LoadScene("Level3");
        }
    }
}

