using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //The scene needs a MenuManager object with this script attached
    //and a GameObject with the PauseMenu prefab assigned to pauseMenu in the inspector.
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameMenu;

    private bool isPaused = false;

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if(gameMenu != null)
        {
            gameMenu.SetActive(!isPaused); // Hide game menu when paused
        }
    }

    public void ContinueGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Reset time scale before restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // For editor
#endif
    }
}
