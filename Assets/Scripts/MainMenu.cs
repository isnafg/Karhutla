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
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnExit()
    {
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