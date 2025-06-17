using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject youWinPanel;

    private int playerUnits = 0;
    private int enemyUnits = 0;

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Pastikan UI panel tidak langsung tampil di awal
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (youWinPanel != null) youWinPanel.SetActive(false);
    }

    // Pemanggilan dari Unit.cs saat spawn
    public void RegisterPlayerUnit()
    {
        playerUnits++;
        Debug.Log("✅ RegisterPlayerUnit() dipanggil. Total player unit sekarang: " + playerUnits);
    }

    public void RegisterEnemy()
    {
        enemyUnits++;
        Debug.Log("Enemy Unit Registered. Total: " + enemyUnits);
    }

    // Pemanggilan dari Unit.cs saat mati
    public void UnregisterPlayerUnit()
    {
        playerUnits--;
        Debug.Log("🟥 UnregisterPlayerUnit() dipanggil. Sisa player unit: " + playerUnits);

        if (playerUnits <= 0 && !gameEnded)
        {
            Debug.Log("🔥 Game Over dipanggil!");
            ShowGameOver();
        }
    }

    public void UnregisterEnemy()
    {
        enemyUnits--;
        Debug.Log("Enemy Unit Unregistered. Sisa: " + enemyUnits);

        if (enemyUnits <= 0 && !gameEnded)
        {
            ShowYouWin();
        }
    }

    public void ShowGameOver()
    {
        gameEnded = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Debug.Log("Game Over!");
        Time.timeScale = 0f;
    }

    public void ShowYouWin()
    {
        gameEnded = true;
        if (youWinPanel != null) youWinPanel.SetActive(true);
        Debug.Log("You Win!");
        Time.timeScale = 0f;
    }
}
