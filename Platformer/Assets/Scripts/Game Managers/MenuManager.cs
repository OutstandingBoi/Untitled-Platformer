using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Canvas mainMenu;

    private void Awake()
    {
        mainMenu.enabled = true;
    }

    public void ToggleMenu(Canvas menu)
    {
        mainMenu.enabled = menu.enabled ? true : false;
        menu.enabled = menu.enabled ? false : true;
    }

    public void SelectLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void SetFullscreen (bool isFullsceen)
    {
        Screen.fullScreen = isFullsceen;
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
