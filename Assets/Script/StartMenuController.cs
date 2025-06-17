using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    [Header("References")]
    public GameObject menuCamera;
    public GameObject gameplayCamera;
    public GameObject startMenuPanel;
    public GameObject gameplayElements;
    public GameObject menuMusic;

    private void Start()
    {
        // Tampilkan menu dan kamera menu
        if (startMenuPanel != null)
            startMenuPanel.SetActive(true);

        if (menuCamera != null)
            menuCamera.SetActive(true);

        // Matikan elemen gameplay di awal
        if (gameplayElements != null)
            gameplayElements.SetActive(false);

        // Matikan kamera gameplay di awal
        if (gameplayCamera != null)
            gameplayCamera.SetActive(false);
    }

    public void OnStartClick()
    {
        Time.timeScale = 1f; // Penting untuk menghidupkan kembali game jika sebelumnya di-pause

        if (menuCamera != null)
            menuCamera.SetActive(false);
        if (gameplayCamera != null)
            gameplayCamera.SetActive(true);
        if (startMenuPanel != null)
            startMenuPanel.SetActive(false);
        if (gameplayElements != null)
            gameplayElements.SetActive(true);
        if (menuMusic != null)
            menuMusic.GetComponent<MenuMusic>().StopMusic();
    }


    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
