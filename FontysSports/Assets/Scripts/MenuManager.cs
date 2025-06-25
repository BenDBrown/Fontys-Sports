using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameMenu;

    private bool isPaused = false;
    private bool lastMenuButtonState = false;

    void Update()
    {
        // Check Menu button press on the right controller
        var rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        bool menuButtonPressed = false;

        if (rightHand.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonPressed))
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
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
