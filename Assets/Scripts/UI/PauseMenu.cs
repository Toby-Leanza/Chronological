using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject controlsMenu;
    public GameObject playerUI;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);
        playerUI.SetActive(!isPaused);

        Time.timeScale = isPaused ? 0 : 1;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void OpenControls()
    {
        controlsMenu.SetActive(true);
        settingsMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseControls()
    {
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
