using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float zOffset = -6f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.z = target.position.z + zOffset;
        transform.position = pos;
    }
}