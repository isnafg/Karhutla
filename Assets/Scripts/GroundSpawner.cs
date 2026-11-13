using UnityEngine;
using System.Collections.Generic;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundPrefab;
    public Transform player;
    public Material forestMaterial;
    public Material burntMaterial;
    public Material villageMaterial;
    public float pieceLength = 20f;
    public float spawnAhead = 100f;
    public float removeBehind = 30f;

    float nextZ = 20f;
    Queue<GameObject> pieces = new Queue<GameObject>();

    void Update()
    {
        // Add new pieces in front of the player
        while (nextZ < player.position.z + spawnAhead)
        {
            GameObject piece = Instantiate(groundPrefab, new Vector3(0f, 0f, nextZ), Quaternion.identity);

            // Pick the ground material for the zone this piece is in
            Renderer r = piece.GetComponent<Renderer>();
            ZoneType zone = ZoneTrack.Instance.TypeAt(nextZ);
            if (zone == ZoneType.Village)
            {
                r.sharedMaterial = villageMaterial;
            }
            else if (zone == ZoneType.Wildfire)
            {
                r.sharedMaterial = burntMaterial;
            }
            else
            {
                r.sharedMaterial = forestMaterial;
            }

            pieces.Enqueue(piece);
            nextZ += pieceLength;
        }

        // Remove the oldest piece once it is far behind the player
        if (pieces.Count > 0 && pieces.Peek().transform.position.z < player.position.z - removeBehind)
        {
            Destroy(pieces.Dequeue());
        }
    }
}