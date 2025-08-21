using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText; // Thêm để hiển thị phần trăm
    [SerializeField] private GunRaycasting gunRaycasting;

    void Start()
    {
        currentHealth = maxHealth;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (healthBar != null) healthBar.value = currentHealth / maxHealth;
        if (healthText != null) healthText.text = $"HP: {Mathf.RoundToInt(currentHealth)}%";
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthBar != null) healthBar.value = currentHealth / maxHealth;
        if (healthText != null) healthText.text = $"HP: {Mathf.RoundToInt(currentHealth)}%";
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gunRaycasting != null) gunRaycasting.HideAllCrosshairs();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor visible: " + Cursor.visible);
    }
}