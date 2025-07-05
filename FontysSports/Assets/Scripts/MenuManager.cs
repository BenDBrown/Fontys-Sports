#nullable enable
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject? gameMenu;
    [SerializeField] private GameObject? mainMenu;
    [SerializeField] private GameObject? gameSelectionMenu;

    private bool isPaused = false;
    private bool lastMenuButtonState = false;
    private SceneLoader sceneLoader;

    void Start()
    {
        if (pauseMenu == null)
        {
            Debug.LogError("Pause menu not assigned. Please assign it in the inspector.");
            return;
        }

        pauseMenu.SetActive(false);

        if (gameMenu != null)
        {
            gameMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Game menu not assigned. It will not be toggled.");
        }
    }

    void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            return;
        }

        // Check XR menu button
        var leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool menuButtonPressed = false;

        if (leftHand.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonPressed))
        {
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

    public void SelectionMenu()
    {
        if (gameSelectionMenu != null)
        {
            gameSelectionMenu.SetActive(false);
        }
        else
        {
            Debug.LogError("Game selection menu not assigned. Please assign it in the inspector.");
        }

        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Main menu not assigned. It will not be toggled.");
        }
    }
}
