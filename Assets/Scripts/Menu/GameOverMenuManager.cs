using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuManager : MonoBehaviour
{

    public void TryAgain()
    {
        Time.timeScale = 1f;

        // TODO: Reload the current scene when level management is implemented
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}