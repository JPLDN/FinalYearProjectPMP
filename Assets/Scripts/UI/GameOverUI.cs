using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    void Awake()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Game Over Panel is hidden until player dies
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true); // Show Game Over Panel when player dies

        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; // Resume the game before leaving the scene

        SceneManager.LoadScene("MainMenu"); // Load the Main Menu scene 
    }
}

