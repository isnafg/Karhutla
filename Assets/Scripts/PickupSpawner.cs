using UnityEngine;
using System.Collections.Generic;

public class PickupSpawner : MonoBehaviour
{
    public GameObject bananaPrefab;
    public GameObject packPrefab;
    public GameObject medicinePrefab;
    public Transform player;
    public float laneWidth = 2f;
    public float placeAhead = 70f;
    public float removeBehind = 30f;

    // The forest has full air, so pickups are rarer there
    public float forestMinGap = 40f;
    public float forestMaxGap = 80f;
    public float otherMinGap = 20f;
    public float otherMaxGap = 40f;

    // Pickups are never placed on an obstacle or hidden right behind one
    public float clearFront = 4f;
    public float clearBehind = 12f;
    public float sideMargin = 0.6f;

    float nextZ = 45f;
    ObstacleSpawner obstacleSpawner;
    Queue<GameObject> items = new Queue<GameObject>();

    void Start()
    {
        obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();
    }

    void Update()
    {
        // Add new pickups in front of the player
        while (nextZ < player.position.z + placeAhead)
        {
            ZoneType zone = ZoneTrack.Instance.TypeAt(nextZ);

            // Pick which pickup to spawn (the medicine only exists in the wildfire)
            GameObject prefab = bananaPrefab;
            float roll = Random.value;
            if (zone == ZoneType.Wildfire)
            {
                if (roll < 0.15f)
                {
                    prefab = medicinePrefab;
                }
                else if (roll < 0.40f)
                {
                    prefab = packPrefab;
                }
            }
            else if (roll < 0.20f)
            {
                prefab = packPrefab;
            }

            // Try the three lanes in random order and take the first one that is free
            bool placed = false;
            int start = Random.Range(-1, 2);
            for (int k = 0; k < 3; k++)
            {
                int lane = ((start + 1 + k) % 3) - 1;
                float x = lane * laneWidth;
                bool blocked = obstacleSpawner != null && obstacleSpawner.IsBlocked(x, nextZ, clearFront, clearBehind, sideMargin);
                if (!blocked)
                {
                    float y = prefab.transform.position.y;
                    items.Enqueue(Instantiate(prefab, new Vector3(x, y, nextZ), Quaternion.identity));
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                // Every lane is blocked here, so try again a little further on
                nextZ += 6f;
            }
            else if (zone == ZoneType.Forest)
            {
                nextZ += Random.Range(forestMinGap, forestMaxGap);
            }
            else
            {
                nextZ += Random.Range(otherMinGap, otherMaxGap);
            }
        }

        // Remove pickups that are far behind the player
        while (items.Count > 0 && (items.Peek() == null || items.Peek().transform.position.z < player.position.z - removeBehind))
        {
            GameObject old = items.Dequeue();
            if (old != null)
            {
                Destroy(old);
            }
        }
    }
}