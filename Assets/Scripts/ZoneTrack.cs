using UnityEngine;
using System.Collections.Generic;

public enum ZoneType
{
    Forest,
    Wildfire,
    Village
}

public class ZoneTrack : MonoBehaviour
{
    public static ZoneTrack Instance;

    public float firstZoneLength = 370f;
    public float minZoneLength = 300f;
    public float maxZoneLength = 500f;
    public float pieceLength = 20f;
    public bool allowSameZoneTwice = false;

    List<ZoneType> types = new List<ZoneType>();
    List<float> starts = new List<float>();
    float generatedUntil = 0f;

    void Awake()
    {
        Instance = this;
        AddZone(ZoneType.Forest, firstZoneLength);
    }

    void AddZone(ZoneType type, float length)
    {
        types.Add(type);
        starts.Add(generatedUntil);
        generatedUntil += length;
    }

    void AddRandomZone()
    {
        ZoneType last = types[types.Count - 1];
        ZoneType next = (ZoneType)Random.Range(0, 3);
        while (!allowSameZoneTwice && next == last)
        {
            next = (ZoneType)Random.Range(0, 3);
        }

        // Zone lengths are multiples of the ground piece length, so zones always start at a piece edge
        float length = Random.Range(minZoneLength, maxZoneLength);
        length = Mathf.Max(pieceLength, Mathf.Round(length / pieceLength) * pieceLength);
        AddZone(next, length);
    }

    public int IndexAt(float z)
    {
        // Create zones on demand, as far as needed
        while (generatedUntil <= z)
        {
            AddRandomZone();
        }
        for (int i = types.Count - 1; i >= 0; i--)
        {
            if (z >= starts[i])
            {
                return i;
            }
        }
        return 0;
    }

    public ZoneType TypeAt(float z)
    {
        return types[IndexAt(z)];
    }

    public ZoneType TypeOf(int index)
    {
        return types[Mathf.Clamp(index, 0, types.Count - 1)];
    }

    public float StartOf(int index)
    {
        return starts[index];
    }
}