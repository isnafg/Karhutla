using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "SampleScene";
    public TextMeshProUGUI bestText;
    public GameObject exitMessage;

    void Start()
    {
        Time.timeScale = 1f;
        bestText.text = "Best: " + PlayerPrefs.GetInt("BestDistance", 0) + " m";
        exitMessage.SetActive(false);
    }

    public void OnPlay()
    {
        if (AudioManager.Instance != null) { AudioManager.Instance.PlayClick(); }
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnExit()
    {
        if (AudioManager.Instance != null) { AudioManager.Instance.PlayClick(); }
#if UNITY_WEBGL && !UNITY_EDITOR
        // A browser game can't close its own tab, so show a goodbye message instead
        exitMessage.SetActive(true);
#elif UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}