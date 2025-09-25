using UnityEngine;
using UnityEngine.SceneManagement; // For loading scenes

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign Pause Menu Canvas in Inspector
    public GameObject GameUI;
    public static bool isPaused = false;

    void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameUI.SetActive(true);
        Time.timeScale = 1f; // Resume time
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameUI.SetActive(false);
        Time.timeScale = 0f; // Freeze time
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reset time before loading
        SceneManager.LoadScene("HomePage"); // Replace with your main menu scene name
    }

    public void ExitGame()
    {
        Time.timeScale = 1f; // Reset time before quitting
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
