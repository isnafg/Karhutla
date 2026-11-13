using UnityEngine;

public class CrashDetector : MonoBehaviour
{
    public GameObject pickupEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Bullet"))
        {
            // A bullet kills instantly, with no extra chances
            GameManager.Instance.GameOver();
        }
        else if (other.name.StartsWith("Obstacle"))
        {
            // Every other obstacle: costs air and counts as one of the three hits
            other.enabled = false;
            FireChase.Instance.Stumble();
        }
        else
        {
            // Pickups refill the air bar and burst into particles
            Pickup pickup = other.GetComponent<Pickup>();
            if (pickup != null)
            {
                AirSystem.Instance.AddAir(pickup.airAmount);
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, other.transform.position, Quaternion.identity);
                }
                other.gameObject.SetActive(false);
            }
        }
    }
}