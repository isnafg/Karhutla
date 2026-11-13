using UnityEngine;

public class BulletMover : MonoBehaviour
{
    public float speed = 20f;

    void Start()
    {
        Destroy(gameObject, 8f);
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
    }
}