using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject creditsMenu;
    public GameObject mainMenu;

    public void OpenCreditsMenu()
    {
        creditsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void CloseCreditsMenu()
    {
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("General");
    }

}
