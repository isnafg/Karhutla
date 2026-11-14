using UnityEngine;
using System.Collections.Generic;

public class SceneryManager : MonoBehaviour
{
    public GameObject[] forestPrefabs;
    public GameObject[] burntPrefabs;
    public GameObject[] housePrefabs;
    public Transform player;
    public float groundY = 0.25f;
    public float spawnAhead = 100f;
    public float removeBehind = 30f;
    public float minGap = 4f;
    public float maxGap = 9f;
    public float minSide = 5f;
    public float maxSide = 10f;

    float nextZ = 10f;
    bool leftSide = false;
    Queue<GameObject> items = new Queue<GameObject>();

    void Update()
    {
        // Add scenery in front of the player, alternating left and right
        while (nextZ < player.position.z + spawnAhead)
        {
            ZoneType zone = ZoneTrack.Instance.TypeAt(nextZ);
            GameObject[] list = forestPrefabs;
            if (zone == ZoneType.Village)
            {
                list = housePrefabs;
            }
            else if (zone == ZoneType.Wildfire)
            {
                list = burntPrefabs;
            }
            GameObject prefab = list[Random.Range(0, list.Length)];

            float x = Random.Range(minSide, maxSide);
            if (leftSide)
            {
                x = -x;
            }
            leftSide = !leftSide;

            // Houses keep their rotation, trees turn randomly
            Quaternion rot = Quaternion.identity;
            if (zone != ZoneType.Village)
            {
                rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            }

            GameObject item = Instantiate(prefab, new Vector3(x, groundY, nextZ), rot);
            item.transform.localScale *= Random.Range(0.8f, 1.3f);

            // Move it so the lowest point of the model touches the ground
            bool found = false;
            Bounds bounds = new Bounds();
            foreach (Renderer r in item.GetComponentsInChildren<Renderer>())
            {
                if (!r.enabled)
                {
                    continue;
                }
                if (!found)
                {
                    bounds = r.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(r.bounds);
                }
            }
            if (found)
            {
                item.transform.position += Vector3.up * (groundY - bounds.min.y);
            }

            items.Enqueue(item);
            nextZ += Mathf.Max(1f, Random.Range(minGap, maxGap));
        }

        // Remove scenery that is far behind the player
        while (items.Count > 0 && items.Peek().transform.position.z < player.position.z - removeBehind)
        {
            Destroy(items.Dequeue());
        }
    }
}