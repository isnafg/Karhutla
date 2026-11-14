using UnityEngine;

public class FireAmbience : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        if (AudioManager.Instance == null || player == null)
        {
            return;
        }

        bool inWildfire = ZoneTrack.Instance.TypeAt(player.position.z) == ZoneType.Wildfire;
        bool playing = Time.timeScale > 0f && !GameManager.Instance.IsOver;
        AudioManager.Instance.SetFireAmbience(inWildfire && playing);
    }

    void OnDisable()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetFireAmbience(false);
        }
    }
}