using UnityEngine;
using UnityEngine.UI;

public class AirSystem : MonoBehaviour
{
    public static AirSystem Instance;

    public Transform player;
    public Slider bar;
    public float maxAir = 100f;
    public float forestDrain = 0f;
    public float wildfireDrain = 3f;
    public float villageDrain = 1.5f;
    public float redBelow = 31f;
    public float yellowBelow = 51f;

    float air;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        air = maxAir;
    }

    void Update()
    {
        // Pick the drain speed for the zone the player is in
        ZoneType zone = ZoneTrack.Instance.TypeAt(player.position.z);
        float drain = forestDrain;
        if (zone == ZoneType.Wildfire)
        {
            drain = wildfireDrain;
        }
        else if (zone == ZoneType.Village)
        {
            drain = villageDrain;
        }

        air -= drain * Time.deltaTime;
        air = Mathf.Clamp(air, 0f, maxAir);
        bar.value = air / maxAir;

        // Low air sound: quiet when yellow, loud when red, silent in the forest and when paused
        float level = 0f;
        if (zone != ZoneType.Forest && Time.timeScale > 0f)
        {
            if (air < redBelow)
            {
                level = 1f;
            }
            else if (air < yellowBelow)
            {
                level = 0.5f;
            }
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetLowAir(level);
        }

        if (air <= 0f)
        {
            GameManager.Instance.GameOver();
        }
    }

    void OnDestroy()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetLowAir(0f);
        }
    }

    public void AddAir(float amount)
    {
        air = Mathf.Min(air + amount, maxAir);
    }
}