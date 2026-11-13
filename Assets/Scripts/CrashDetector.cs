using UnityEngine;

public class CrashDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Obstacle"))
        {
            // Switch the obstacle off so it only counts once
            other.enabled = false;
            FireChase.Instance.Stumble();
        }
        else
        {
            // Pickups refill the air bar
            Pickup pickup = other.GetComponent<Pickup>();
            if (pickup != null)
            {
                AirSystem.Instance.AddAir(pickup.airAmount);
                other.gameObject.SetActive(false);
            }
        }
    }
}