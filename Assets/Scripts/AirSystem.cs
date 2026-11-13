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

        if (air <= 0f)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void AddAir(float amount)
    {
        air = Mathf.Min(air + amount, maxAir);
    }
}