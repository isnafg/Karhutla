using UnityEngine;

public class FollowZ : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        Vector3 p = transform.position;
        p.z = target.position.z;
        transform.position = p;
    }
}