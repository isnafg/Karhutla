using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] forestObstacles;
    public GameObject[] wildfireObstacles;
    public GameObject[] villageObstacles;
    public Transform player;
    public float laneWidth = 2f;
    public float twoLaneChance = 0.5f;
    public float spawnAhead = 100f;
    public float removeBehind = 30f;
    public float minGap = 15f;
    public float maxGap = 30f;
    public bool logSpawns = false;

    float nextZ = 40f;
    string lastSignature = "";
    List<float> xs = new List<float>();
    Queue<GameObject> obstacles = new Queue<GameObject>();

    void Update()
    {
        // Add new obstacles in front of the player
        while (nextZ < player.position.z + spawnAhead)
        {
            // Pick the obstacle list for the zone this spot is in
            ZoneType zone = ZoneTrack.Instance.TypeAt(nextZ);
            GameObject[] set = forestObstacles;
            if (zone == ZoneType.Village && villageObstacles.Length > 0)
            {
                set = villageObstacles;
            }
            else if (zone == ZoneType.Wildfire && wildfireObstacles.Length > 0)
            {
                set = wildfireObstacles;
            }

            SpawnVariation(set, nextZ);
            nextZ += Random.Range(minGap, maxGap);
        }

        // Remove obstacles that are far behind the player
        while (obstacles.Count > 0 && obstacles.Peek().transform.position.z < player.position.z - removeBehind)
        {
            Destroy(obstacles.Dequeue());
        }
    }

    void SpawnVariation(GameObject[] set, float z)
    {
        GameObject prefab = null;
        string signature = "";

        // Roll again until it differs from the previous obstacle (type, placement and quantity)
        for (int attempt = 0; attempt < 30; attempt++)
        {
            prefab = set[Random.Range(0, set.Length)];
            signature = RollVariant(prefab);
            if (signature != lastSignature)
            {
                break;
            }
        }

        lastSignature = signature;
        if (logSpawns)
        {
            Debug.Log("Spawn " + signature);
        }

        float y = prefab.transform.position.y;
        for (int i = 0; i < xs.Count; i++)
        {
            obstacles.Enqueue(Instantiate(prefab, new Vector3(xs[i], y, z), Quaternion.identity));
        }
    }

    // Picks the lanes for a prefab and returns its signature: type | placement | quantity
    string RollVariant(GameObject prefab)
    {
        xs.Clear();

        if (prefab.name.Contains("Bridge"))
        {
            // The bridge covers all three lanes
            xs.Add(0f);
            return prefab.name + "|all|3";
        }

        if (prefab.name.Contains("Tree"))
        {
            // The tree is two lanes wide: left + middle, or middle + right
            int side = Random.value < 0.5f ? -1 : 1;
            xs.Add(side * laneWidth / 2f);
            return prefab.name + "|" + side + "|2";
        }

        if (Random.value < twoLaneChance)
        {
            // Two copies side by side: left + middle (-1), or middle + right (0)
            int first = Random.Range(-1, 1);
            xs.Add(first * laneWidth);
            xs.Add((first + 1) * laneWidth);
            return prefab.name + "|" + first + "|2";
        }

        // One copy in a random lane
        int lane = Random.Range(-1, 2);
        xs.Add(lane * laneWidth);
        return prefab.name + "|" + lane + "|1";
    }

    // True if a pickup at (x, z) would sit inside an obstacle, just in front of it, or hidden right behind it
    public bool IsBlocked(float x, float z, float clearFront, float clearBehind, float sideMargin)
    {
        foreach (GameObject o in obstacles)
        {
            if (o == null)
            {
                continue;
            }

            BoxCollider bc = o.GetComponent<BoxCollider>();
            if (bc == null || !bc.enabled)
            {
                continue;
            }

            Vector3 center = o.transform.TransformPoint(bc.center);
            Vector3 half = Vector3.Scale(bc.size, o.transform.lossyScale) * 0.5f;
            half = new Vector3(Mathf.Abs(half.x), Mathf.Abs(half.y), Mathf.Abs(half.z));

            bool sameLane = Mathf.Abs(x - center.x) < half.x + sideMargin;
            bool tooClose = z > center.z - half.z - clearFront && z < center.z + half.z + clearBehind;
            if (sameLane && tooClose)
            {
                return true;
            }
        }
        return false;
    }

    // True if a live obstacle is between two world Z positions (the gunshots use this)
    public bool HasObstacleBetween(float zMin, float zMax)
    {
        foreach (GameObject o in obstacles)
        {
            if (o == null)
            {
                continue;
            }

            float oz = o.transform.position.z;
            if (oz < zMin || oz > zMax)
            {
                continue;
            }

            // An obstacle you already hit has its collider switched off, so it no longer counts
            Collider c = o.GetComponent<Collider>();
            if (c == null || c.enabled)
            {
                return true;
            }
        }
        return false;
    }
}