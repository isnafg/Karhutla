using UnityEngine;
using UnityEngine.UI;

public class FireChase : MonoBehaviour
{
    public static FireChase Instance;

    public Image glow;
    public float airPenalty = 5f;
    public int hitsToDie = 3;
    public float calmTime = 10f;
    public float maxGlow = 0.6f;

    int hits = 0;
    float calmTimer = 0f;
    float glowAlpha = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // The fire calms down if you avoid obstacles long enough
        if (hits > 0)
        {
            calmTimer -= Time.deltaTime;
            if (calmTimer <= 0f)
            {
                hits = 0;
            }
        }

        // The more hits, the stronger the orange glow (it fades in and out smoothly)
        float target = (float)hits / hitsToDie * maxGlow;
        glowAlpha = Mathf.MoveTowards(glowAlpha, target, Time.deltaTime * 0.5f);
        Color c = glow.color;
        c.a = glowAlpha;
        glow.color = c;
    }

    // Called by the crash detector when you hit an obstacle
    public void Stumble()
    {
        hits++;
        calmTimer = calmTime;
        AirSystem.Instance.AddAir(-airPenalty);
        if (PostFx.Instance != null)
        {
            PostFx.Instance.Flash();
        }

        if (hits >= hitsToDie)
        {
            GameManager.Instance.GameOver();
        }
    }
}