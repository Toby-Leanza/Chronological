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
    private bool inSubMenu = false;

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Update()
    {
        // SOLUCIÓN: Verificar si está pausado Y en submenú
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Solo bloquear la tecla P si está pausado Y en submenú
            if (!isPaused || !inSubMenu)
            {
                TogglePause();
            }
        }

        // Tecla Escape para volver atrás desde submenús
        if (Input.GetKeyDown(KeyCode.Escape) && inSubMenu)
        {
            HandleBackButton();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);
        playerUI.SetActive(!isPaused);

        // Si estamos despausando, cerrar todos los submenús
        if (!isPaused)
        {
            settingsMenu.SetActive(false);
            controlsMenu.SetActive(false);
            inSubMenu = false;
        }

        Time.timeScale = isPaused ? 0 : 1;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        PlayButtonClick();
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        inSubMenu = true;
        PlayButtonClick();
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        inSubMenu = false;
        PlayButtonClick();
    }

    public void OpenControls()
    {
        controlsMenu.SetActive(true);
        settingsMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        inSubMenu = true;
        PlayButtonClick();
    }

    public void CloseControls()
    {
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        // inSubMenu sigue siendo true porque aún está en settings
        PlayButtonClick();
    }

    public void ResumeGame()
    {
        // Forzar el cierre de todos los menús
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        playerUI.SetActive(true);

        inSubMenu = false;
        isPaused = false;

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;

        PlayButtonClick();
    }

    private void HandleBackButton()
    {
        if (controlsMenu.activeSelf)
        {
            CloseControls();
        }
        else if (settingsMenu.activeSelf)
        {
            CloseSettings();
        }
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