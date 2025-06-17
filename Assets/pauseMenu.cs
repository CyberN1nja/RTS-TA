using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;    // Drag PausePanel ke sini dari Inspector
    public GameObject pauseButton;   // Tombol pause di pojok

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // Sembunyikan panel saat mulai
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(true); // Pastikan tombol pause aktif di awal
        }
    }

    void Update()
    {
        if (isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Memberhentikan waktu (pause)
        isPaused = true;

        if (pausePanel != null) pausePanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Melanjutkan waktu (resume)
        isPaused = false;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Normalisasi waktu sebelum reload
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToHome()
    {
        Time.timeScale = 1f; // Normalisasi waktu sebelum pindah scene
        SceneManager.LoadScene("MainMenu"); // Ubah jika nama scene utama kamu berbeda
    }
}
