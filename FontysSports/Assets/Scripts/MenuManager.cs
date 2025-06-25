using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameMenu;

    private bool isPaused = false;
    private bool lastMenuButtonState = false;
    private SceneLoader sceneLoader;

    void Update()
    {
        // Check Menu button press on the right controller
        var leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool menuButtonPressed = false;

        if (leftHand.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonPressed))
        {
            // Toggle pause only on button down (not every frame it's held)
            if (menuButtonPressed && !lastMenuButtonState)
            {
                TogglePause();
            }
            lastMenuButtonState = menuButtonPressed;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if (gameMenu != null)
        {
            gameMenu.SetActive(!isPaused);
        }
    }

    public void ContinueGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

        if (gameMenu != null)
        {
            gameMenu.SetActive(true);
        }
        else
        {
            Debug.LogError("Game menu not found. Please assign it in the inspector.");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
