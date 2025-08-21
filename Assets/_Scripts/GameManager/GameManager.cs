using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private int enemiesKilled = 0;
    [SerializeField] private int initialEnemiesToKill = 5;
    [SerializeField] private TextMeshProUGUI taskText;
    [SerializeField] private GameObject taskCompletePanel;
    [SerializeField] private GunRaycasting gunRaycasting;

    private List<GameObject> activeLargeDrones = new List<GameObject>();

    void Start()
    {
        if (taskText != null)
        {
            taskText.text = $"Nhiệm vụ: Tiêu diệt {initialEnemiesToKill - enemiesKilled} drone lớn";
        }
        else
        {
            Debug.LogError("taskText not assigned in GameManager!");
        }
        if (taskCompletePanel != null)
        {
            taskCompletePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("taskCompletePanel not assigned in GameManager!");
        }
        activeLargeDrones.Clear();
        foreach (var drone in FindObjectsOfType<DroneAI>())
        {
            if (drone.gameObject.activeInHierarchy)
            {
                activeLargeDrones.Add(drone.gameObject);
            }
        }
        initialEnemiesToKill = activeLargeDrones.Count;
        UpdateTaskUI();
        Debug.Log("Initial large drones: " + initialEnemiesToKill + ", Found drones: " + string.Join(", ", activeLargeDrones.Select(d => d.name)));
    }

    public void EnemyKilled(GameObject drone)
    {
        if (drone.GetComponent<DroneAI>() != null)
        {
            enemiesKilled++;
            activeLargeDrones.Remove(drone);
            Debug.Log("Large drone killed: " + drone.name + ", Enemies killed: " + enemiesKilled + ", Remaining large drones: " + activeLargeDrones.Count);
            UpdateTaskUI();
            if (activeLargeDrones.Count == 0)
            {
                if (taskCompletePanel != null)
                {
                    taskCompletePanel.SetActive(true);
                    taskText.enabled = false;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    if (gunRaycasting != null)
                    {
                        gunRaycasting.HideAllCrosshairs();
                    }
                    else
                    {
                        Debug.LogError("gunRaycasting not assigned in GameManager!");
                    }
                    Debug.Log("Cursor visible: " + Cursor.visible);
                }
                Time.timeScale = 0f;
                Debug.Log("Nhiệm vụ hoàn thành!");
            }
        }
        else if (drone.GetComponent<SmallDroneAI>() != null)
        {
            Debug.Log("Small drone killed: " + drone.name + ", not counted for mission");
        }
    }

    private void UpdateTaskUI()
    {
        if (taskText != null)
        {
            taskText.text = $"Nhiệm vụ: Tiêu diệt tất cả drone lớn ({activeLargeDrones.Count} còn lại)";
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit");
    }
}