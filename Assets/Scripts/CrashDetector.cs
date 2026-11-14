using UnityEngine;

public class CrashDetector : MonoBehaviour
{
    public GameObject pickupEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Bullet"))
        {
            if (AudioManager.Instance != null) { AudioManager.Instance.PlayBulletHit(); }
            GameManager.Instance.GameOver();
        }
        else if (other.name.StartsWith("Obstacle"))
        {
            if (AudioManager.Instance != null)
            {
                if (other.name.Contains("Fire")) { AudioManager.Instance.PlayFireHit(); }
                else { AudioManager.Instance.PlayObstacleHit(); }
            }
            other.enabled = false;
            FireChase.Instance.Stumble();
        }
        else
        {
            Pickup pickup = other.GetComponent<Pickup>();
            if (pickup != null)
            {
                AirSystem.Instance.AddAir(pickup.airAmount);
                if (AudioManager.Instance != null) { AudioManager.Instance.PlayPickup(); }
                if (pickupEffect != null) { Instantiate(pickupEffect, other.transform.position, Quaternion.identity); }
                other.gameObject.SetActive(false);
            }
        }
    }
}