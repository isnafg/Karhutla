using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform player;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI gameOverText;
    public GameObject gameOverPanel;
    public GameObject airBar;

    bool isOver = false;
    float startZ;
    int meters = 0;
    int best = 0;
    public bool IsOver { get { return isOver; } }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        startZ = player.position.z;
        best = PlayerPrefs.GetInt("BestDistance", 0);
        bestText.text = "Best: " + best + " m";
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (!isOver)
        {
            meters = Mathf.FloorToInt(player.position.z - startZ);
            scoreText.text = meters + " m";
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void GameOver()
    {
        if (isOver)
        {
            return;
        }
        isOver = true;

        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.PlayGameOver();
        }

        // Save a new best distance
        bool newBest = meters > best;
        if (newBest)
        {
            best = meters;
            PlayerPrefs.SetInt("BestDistance", best);
            PlayerPrefs.Save();
        }

        scoreText.gameObject.SetActive(false);
        bestText.gameObject.SetActive(false);
        airBar.SetActive(false);

        string bestLine = newBest ? "New best!" : "Best: " + best + " m";
        gameOverText.text = "<size=70>Game Over</size>\n<size=44>" + meters + " m</size>\n<size=28>" + bestLine + "\nPress R to restart</size>";
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}