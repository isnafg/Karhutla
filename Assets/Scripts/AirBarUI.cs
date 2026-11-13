using UnityEngine;
using UnityEngine.UI;

public class AirBarUI : MonoBehaviour
{
    public Transform player;
    public Image[] glows;
    public float maxAir = 100f;
    public float redBelow = 31f;
    public float yellowBelow = 51f;
    public Color green = new Color(0.30f, 0.85f, 0.39f);
    public Color blue = new Color(0.25f, 0.66f, 0.96f);
    public Color yellow = new Color(1f, 0.80f, 0f);
    public Color red = new Color(1f, 0.23f, 0.19f);
    public float maxGlowAlpha = 0.55f;

    Slider slider;
    Image fill;

    void Start()
    {
        slider = GetComponent<Slider>();
        fill = slider.fillRect.GetComponent<Image>();

        // Make the fill cover the whole bar by removing the padding Unity adds by default
        RectTransform fillArea = slider.fillRect.parent as RectTransform;
        if (fillArea != null)
        {
            fillArea.anchorMin = Vector2.zero;
            fillArea.anchorMax = Vector2.one;
            fillArea.offsetMin = Vector2.zero;
            fillArea.offsetMax = Vector2.zero;
        }
        slider.fillRect.offsetMin = Vector2.zero;
        slider.fillRect.offsetMax = Vector2.zero;

        // Make the dark background the same size as the whole bar
        RectTransform bg = transform.Find("Background") as RectTransform;
        if (bg != null)
        {
            bg.anchorMin = Vector2.zero;
            bg.anchorMax = Vector2.one;
            bg.offsetMin = Vector2.zero;
            bg.offsetMax = Vector2.zero;
        }
    }

    void Update()
    {
        float air = slider.value * maxAir;
        bool inForest = ZoneTrack.Instance.TypeAt(player.position.z) == ZoneType.Forest;

        Color c = blue;
        float speed = 1.5f;
        bool showGlow = false;

        if (inForest)
        {
            // Immune in the forest: always green with a flickering green glow, whatever the air is
            c = green;
            speed = 3f;
            showGlow = true;
        }
        else if (air < redBelow)
        {
            c = red;
            speed = 6f;
            showGlow = true;
        }
        else if (air < yellowBelow)
        {
            c = yellow;
            speed = 3f;
            showGlow = true;
        }

        fill.color = c;

        // The glow flickers, and the outer layers are fainter than the inner ones
        float flicker = 0.55f + 0.45f * Mathf.Sin(Time.time * speed);
        float alpha = showGlow ? maxGlowAlpha * flicker : 0f;
        for (int i = 0; i < glows.Length; i++)
        {
            Color g = c;
            g.a = alpha * (i + 1f) / glows.Length;
            glows[i].color = g;
        }
    }
}