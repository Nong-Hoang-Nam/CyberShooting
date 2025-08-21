using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GunRaycasting gunRaycasting;

    private bool isPaused = false;

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        isPaused = !isPaused;
        if (settingsPanel != null) settingsPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
        if (gunRaycasting != null)
        {
            if (isPaused) gunRaycasting.HideAllCrosshairs();
            else gunRaycasting.ShowAllCrosshairs();
        }
        Debug.Log("Settings panel " + (isPaused ? "shown" : "hidden") + ", Cursor visible: " + Cursor.visible);
    }

    public void ShowHowToPlay()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(true);
    }

    public void HideHowToPlay()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        Debug.Log("Volume set to: " + volume);
    }
}