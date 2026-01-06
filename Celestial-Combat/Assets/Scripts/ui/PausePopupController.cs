
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePopupController : MonoBehaviour
{
    public GameObject pauseOverlay;

    public void OpenPause()
    {
        pauseOverlay.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseOverlay.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
