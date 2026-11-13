using UnityEngine;

public class EmbersController : MonoBehaviour
{
    public Transform player;
    public float lookAhead = 15f;

    ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Follow the player along the track
        transform.position = new Vector3(0f, 0f, player.position.z);

        // Embers only appear in the wildfire
        bool inWildfire = ZoneTrack.Instance.TypeAt(player.position.z + lookAhead) == ZoneType.Wildfire;
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.enabled = inWildfire;
    }
}