using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PostFx : MonoBehaviour
{
    public static PostFx Instance;

    public Volume volume;
    public Slider airBar;
    public Transform player;

    public Color forestColor = new Color(0.20f, 0.85f, 0.35f);
    public Color dangerColor = new Color(0.90f, 0.05f, 0.05f);

    // Forest: always a soft green glow, whatever the air is
    public float forestIntensity = 0.38f;

    // Wildfire and village: red vignette that starts at half air and grows as the air drops
    public float dangerStartAir = 0.5f;
    public float dangerMinIntensity = 0.30f;
    public float dangerMaxIntensity = 0.65f;

    public float smoothness = 1f;
    public float fadeSpeed = 4f;

    // Short red flash when an obstacle is hit
    public float flashIntensity = 0.7f;
    public float flashTime = 0.35f;

    Vignette vignette;
    float flash = 0f;
    float currentIntensity = 0f;
    Color currentColor = new Color(0.20f, 0.85f, 0.35f);

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Using volume.profile makes a copy, so the saved profile file is never changed
        volume.profile.TryGet(out vignette);
        if (vignette == null)
        {
            Debug.LogWarning("PostFx: no Vignette override found in the Global Volume profile");
        }
    }

    void Update()
    {
        if (vignette == null)
        {
            return;
        }

        bool inForest = ZoneTrack.Instance.TypeAt(player.position.z) == ZoneType.Forest;

        Color targetColor = currentColor;
        float targetIntensity = 0f;

        if (inForest)
        {
            // Safe zone: a gently pulsing green glow
            float pulse = 0.85f + 0.15f * Mathf.Sin(Time.time * 2.5f);
            targetColor = forestColor;
            targetIntensity = forestIntensity * pulse;
        }
        else if (airBar.value < dangerStartAir)
        {
            // Low air: the red vignette grows as the air drops
            float danger = Mathf.Clamp01((dangerStartAir - airBar.value) / dangerStartAir);
            float pulse = 0.88f + 0.12f * Mathf.Sin(Time.time * 6f);
            targetColor = dangerColor;
            targetIntensity = Mathf.Lerp(dangerMinIntensity, dangerMaxIntensity, danger) * pulse;
        }

        // Fade smoothly between colors and strengths
        if (targetIntensity > 0f)
        {
            currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * fadeSpeed);
        }
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * fadeSpeed);

        // The hit flash is always red, whatever zone you are in
        flash = Mathf.MoveTowards(flash, 0f, Time.deltaTime / flashTime);
        Color shownColor = Color.Lerp(currentColor, dangerColor, flash);
        float shownIntensity = Mathf.Max(currentIntensity, flash * flashIntensity);

        vignette.active = true;
        vignette.rounded.Override(false);
        vignette.color.Override(shownColor);
        vignette.smoothness.Override(smoothness);
        vignette.intensity.Override(shownIntensity);
    }

    public void Flash()
    {
        flash = 1f;
    }
}