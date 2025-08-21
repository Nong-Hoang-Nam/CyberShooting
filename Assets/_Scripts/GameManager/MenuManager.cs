using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject howToPlayPanel;

    void Start()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit");
    }

    public void ShowHowToPlay()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(true);
    }

    public void HideHowToPlay()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
    }
}