using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.1f;
    public float speed = 12f;

    Vector3 normal;
    Vector3 target;

    void Start()
    {
        normal = transform.localScale;
        target = normal;
    }

    void Update()
    {
        // Unscaled time, so it also works while the game is paused
        transform.localScale = Vector3.Lerp(transform.localScale, target, speed * Time.unscaledDeltaTime);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        target = normal * hoverScale;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHover();
        }
    }

    public void OnPointerExit(PointerEventData e)
    {
        target = normal;
    }

    void OnDisable()
    {
        if (normal != Vector3.zero)
        {
            transform.localScale = normal;
            target = normal;
        }
    }
}