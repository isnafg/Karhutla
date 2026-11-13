using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public Transform player;
    public Camera cam;
    public Color forestColor = new Color(0.294f, 0.651f, 0.925f);
    public Color fireColor = new Color(0.88f, 0.40f, 0.16f);
    public Color villageColor = new Color(0.78f, 0.82f, 0.85f);
    public float skyLead = 80f;
    public float blendLength = 100f;

    Color ColorOf(ZoneType zone)
    {
        if (zone == ZoneType.Wildfire)
        {
            return fireColor;
        }
        if (zone == ZoneType.Village)
        {
            return villageColor;
        }
        return forestColor;
    }

    void Update()
    {
        // The sky reacts to the ground you can already see ahead (the fog distance)
        float lookZ = player.position.z + skyLead;
        ZoneTrack track = ZoneTrack.Instance;
        int i = track.IndexAt(lookZ);

        Color from = ColorOf(track.TypeOf(i - 1));
        Color to = ColorOf(track.TypeOf(i));
        float t = Mathf.Clamp01((lookZ - track.StartOf(i)) / blendLength);
        Color c = Color.Lerp(from, to, t);

        cam.backgroundColor = c;
        RenderSettings.fogColor = c;
    }
}