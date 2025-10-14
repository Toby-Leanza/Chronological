using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject controlsMenu;
    public GameObject playerUI;

    private AudioManager audioManager;
    private bool isPaused = false;

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

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

        // Sonido al pausar/despausar
        PlayButtonClick();
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        PlayButtonClick();
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        PlayButtonClick();
    }

    public void OpenControls()
    {
        controlsMenu.SetActive(true);
        settingsMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        PlayButtonClick();
    }

    public void CloseControls()
    {
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        PlayButtonClick();
    }

    public void QuitGame()
    {
        PlayButtonClick();
        Application.Quit();
    }

    // Métodos para los sonidos
    public void PlayButtonHover()
    {
        if (audioManager != null)
        {
            audioManager.PlayButtonHover();
        }
    }

    public void PlayButtonClick()
    {
        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
        }
    }


}