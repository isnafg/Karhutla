using UnityEngine;
using System.Collections.Generic;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Run cycle")]
    public float cyclesPerSecond = 1.6f;
    public float swing = 35f;
    public float bend = 35f;

    [Header("Roll")]
    public GameObject rollBall;
    public float rollSpeed = 600f;
    public float groundMargin = 0.05f;

    class Part
    {
        public Transform t;
        public Quaternion rest;
        public Vector3 axis;
        public int kind;        // 0 thigh, 1 knee, 2 upper arm, 3 elbow
        public float side;      // +1 or -1, so left and right move opposite
    }

    static readonly string[] partNames = { "UpLeg", "Leg", "Arm", "ForeArm" };

    PlayerRun player;
    SkinnedMeshRenderer skin;
    List<Part> parts = new List<Part>();
    float phase = 0f;

    void Start()
    {
        player = GetComponentInParent<PlayerRun>();
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
        Transform root = player != null ? player.transform : transform;
        Transform[] all = GetComponentsInChildren<Transform>(true);

        // Legs and arms, left and right, all set up in one loop
        for (int kind = 0; kind < 4; kind++)
        {
            for (int i = 0; i < 2; i++)
            {
                string key = (i == 0 ? "Left" : "Right") + partNames[kind];
                foreach (Transform t in all)
                {
                    string n = t.name;
                    if (n.Length > key.Length + 1 && n.StartsWith(key + "_") && char.IsDigit(n[key.Length + 1]))
                    {
                        Part p = new Part();
                        p.t = t;
                        p.rest = t.localRotation;
                        p.kind = kind;
                        p.side = (i == 0 ? 1f : -1f) * (kind >= 2 ? -1f : 1f);
                        p.axis = Quaternion.Inverse(t.rotation) * root.right;
                        parts.Add(p);
                        break;
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        // Frozen while paused or game over
        if (Time.timeScale == 0f) { return; }

        bool rolling = player != null && player.IsSliding;

        // While rolling, the orangutan is hidden and the spinning ball takes its place
        if (skin != null) { skin.enabled = !rolling; }
        if (rollBall != null)
        {
            rollBall.SetActive(rolling);
            if (rolling)
            {
                rollBall.transform.localPosition = new Vector3(0f, groundMargin, 0f);
                rollBall.transform.Rotate(Vector3.right, rollSpeed * Time.deltaTime, Space.World);
            }
        }

        // Running: left and right sides move opposite to each other
        float speed = player != null ? Mathf.Max(0.5f, player.runSpeed / 8f) : 1f;
        phase += Time.deltaTime * cyclesPerSecond * speed * Mathf.PI * 2f;
        float s = Mathf.Sin(phase);
        float c = Mathf.Cos(phase);

        foreach (Part p in parts)
        {
            float angle;
            if (p.kind == 0 || p.kind == 2) { angle = p.side * s * swing; }
            else if (p.kind == 1) { angle = Mathf.Max(0f, p.side * c) * bend; }
            else { angle = -Mathf.Max(0f, p.side * c) * bend; }

            p.t.localRotation = p.rest * Quaternion.AngleAxis(angle, p.axis);
        }
    }
}