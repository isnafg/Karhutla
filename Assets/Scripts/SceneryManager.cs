using UnityEngine;
using System.Collections.Generic;

public class SceneryManager : MonoBehaviour
{
    public GameObject forestPrefab;
    public GameObject burntPrefab;
    public GameObject housePrefab;
    public Transform player;
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
            GameObject prefab = forestPrefab;
            if (zone == ZoneType.Village)
            {
                prefab = housePrefab;
            }
            else if (zone == ZoneType.Wildfire)
            {
                prefab = burntPrefab;
            }

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

            Vector3 pos = new Vector3(x, prefab.transform.position.y, nextZ);
            items.Enqueue(Instantiate(prefab, pos, rot));
            nextZ += Random.Range(minGap, maxGap);
        }

        // Remove scenery that is far behind the player
        while (items.Count > 0 && items.Peek().transform.position.z < player.position.z - removeBehind)
        {
            Destroy(items.Dequeue());
        }
    }
}