using UnityEngine;
using System.Collections;

public class GunshotManager : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public RectTransform[] laneMarks;
    public float laneWidth = 2f;
    public float warningTime = 3f;
    public float minPause = 12f;
    public float maxPause = 20f;
    public float spawnDistance = 45f;
    public float bulletSpeed = 20f;
    public float lowHeight = 0.55f;
    public float highHeight = 1.7f;
    public float safeMargin = 12f;
    public float retryTime = 0.5f;
    public float markHeight = 3.4f;
    public float flickerSpeed = 4f;
    public bool logDebug = false;

    PlayerRun playerRun;
    ObstacleSpawner obstacleSpawner;
    Camera cam;
    CanvasGroup[] groups;
    bool busy = false;
    float pauseTimer = 5f;

    void Start()
    {
        playerRun = player.GetComponent<PlayerRun>();
        obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();
        cam = Camera.main;

        // Each mark gets a CanvasGroup so the whole mark can fade in and out
        groups = new CanvasGroup[laneMarks.Length];
        for (int i = 0; i < laneMarks.Length; i++)
        {
            CanvasGroup g = laneMarks[i].GetComponent<CanvasGroup>();
            if (g == null)
            {
                g = laneMarks[i].gameObject.AddComponent<CanvasGroup>();
            }
            groups[i] = g;
        }
        HideMarks();
    }

    void Update()
    {
        // Hide the warning when the game is over
        if (Time.timeScale == 0f)
        {
            HideMarks();
            return;
        }

        // Gunshots only happen in the village
        if (ZoneTrack.Instance.TypeAt(player.position.z) != ZoneType.Village || busy)
        {
            return;
        }

        pauseTimer -= Time.deltaTime;
        if (pauseTimer <= 0f)
        {
            if (PathIsClear())
            {
                StartCoroutine(Attack());
            }
            else
            {
                // An obstacle is near the spot where the bullets would arrive, so wait a moment
                pauseTimer = retryTime;
                if (logDebug)
                {
                    Debug.Log("Attack delayed: obstacle near the bullet spot");
                }
            }
        }
    }

    // Works out where the bullets will reach the player and checks that no obstacle is near that spot
    bool PathIsClear()
    {
        if (obstacleSpawner == null)
        {
            return true;
        }

        float v = playerRun.runSpeed;
        float hitDistance = v * (warningTime + spawnDistance / (bulletSpeed + v));
        float hitZ = player.position.z + hitDistance;
        return !obstacleSpawner.HasObstacleBetween(hitZ - safeMargin, hitZ + safeMargin);
    }

    void HideMarks()
    {
        for (int i = 0; i < laneMarks.Length; i++)
        {
            laneMarks[i].gameObject.SetActive(false);
        }

        // The warning sound stops the moment the marks are gone
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopWarning();
        }
    }

    // Shows a flickering mark above the player's head in every lane that will get a bullet
    void ShowMarks(int freeLane)
    {
        float flicker = 0.65f + 0.35f * Mathf.Sin(Time.time * flickerSpeed);
        for (int lane = -1; lane <= 1; lane++)
        {
            int i = lane + 1;
            bool danger = lane != freeLane;
            laneMarks[i].gameObject.SetActive(danger);
            if (!danger)
            {
                continue;
            }

            Vector3 world = new Vector3(lane * laneWidth, markHeight, player.position.z);
            Vector3 screen = cam.WorldToScreenPoint(world);
            laneMarks[i].position = new Vector3(screen.x, screen.y, 0f);
            groups[i].alpha = flicker;
        }
    }

    IEnumerator Attack()
    {
        busy = true;

        // One lane is always free
        int freeLane = Random.Range(-1, 2);
        if (logDebug)
        {
            Debug.Log("Attack started, free lane: " + freeLane);
        }

        // Warning: a mark on every lane that will get a bullet
        if (AudioManager.Instance != null) 
        { 
            AudioManager.Instance.PlayWarning(); 
        }
        float t = 0f;
        while (t < warningTime)
        {
            if (Time.timeScale == 0f)
            {
                HideMarks();
                yield return null;
                continue;
            }

            t += Time.deltaTime;
            ShowMarks(freeLane);
            yield return null;
        }
        HideMarks();

        // Two bullets, each randomly high or low
        if (AudioManager.Instance != null) 
        { 
            AudioManager.Instance.PlayGunshot(); 
        }
        for (int lane = -1; lane <= 1; lane++)
        {
            if (lane == freeLane)
            {
                continue;
            }

            bool high = Random.value < 0.5f;
            float y = high ? highHeight : lowHeight;
            Vector3 pos = new Vector3(lane * laneWidth, y, player.position.z + spawnDistance);
            GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
            bullet.GetComponent<BulletMover>().speed = bulletSpeed;
        }

        pauseTimer = Random.Range(minPause, maxPause);
        busy = false;
    }
}