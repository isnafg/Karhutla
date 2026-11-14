using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public string menuSceneName = "MainMenu";

    bool paused = false;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        // You can't pause after the game is over
        if (GameManager.Instance.IsOver)
        {
            return;
        }

        Keyboard kb = Keyboard.current;
        if (kb.escapeKey.wasPressedThisFrame || kb.pKey.wasPressedThisFrame)
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        paused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        if (AudioManager.Instance != null) { AudioManager.Instance.PlayClick(); }
    }

    public void Resume()
    {
        paused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        if (AudioManager.Instance != null) { AudioManager.Instance.PlayResume(); }
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        if (AudioManager.Instance != null) { AudioManager.Instance.PlayClick(); }
        SceneManager.LoadScene(menuSceneName);
    }
}