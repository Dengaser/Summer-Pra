using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    private bool isPaused;
    public static bool IsGamePaused;

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;
        IsGamePaused = false;

        pauseMenuPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SwitchMagic playerMagic = player.GetComponent<SwitchMagic>();
            if (playerMagic != null)
            {

                playerMagic.LoadProgress(resetToLevelStart: true);
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
        IsGamePaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        IsGamePaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}