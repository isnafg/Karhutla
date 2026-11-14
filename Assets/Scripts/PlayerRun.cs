using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRun : MonoBehaviour
{
    public float runSpeed = 8f;
    public float maxSpeed = 18f;
    public float speedGainPerSecond = 0.08f;
    public float laneWidth = 2f;
    public float laneSwitchSpeed = 15f;
    public float jumpSpeed = 9f;
    public float gravity = -25f;
    public float standHeight = 2f;
    public float slideHeight = 1f;
    public float groundCheckExtra = 0.2f;

    [Header("Model")]
    public Transform visual;

    int lane = 0;
    float yVelocity = 0f;
    bool grounded = false;
    float previousHalfHeight = 1f;
    float startSpeed;
    float runTime = 0f;
    bool isSliding = false;
    CapsuleCollider capsule;

    // The animator reads this to know when to roll
    public bool IsSliding { get { return isSliding; } }

    void Start()
    {
        startSpeed = runSpeed;
        capsule = GetComponent<CapsuleCollider>();
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;

        // Nothing moves while the game is paused or over
        if (Time.timeScale == 0f)
        {
            return;
        }

        // 0. Difficulty: the run gets faster the longer you play
        runTime += Time.deltaTime;
        runSpeed = Mathf.Min(startSpeed + runTime * speedGainPerSecond, maxSpeed);

        // 1. Change lane: one key press moves one lane
        if (kb.leftArrowKey.wasPressedThisFrame || kb.aKey.wasPressedThisFrame)
        {
            lane = Mathf.Max(lane - 1, -1);
        }
        if (kb.rightArrowKey.wasPressedThisFrame || kb.dKey.wasPressedThisFrame)
        {
            lane = Mathf.Min(lane + 1, 1);
        }

        // 2. Jump (only possible on the ground)
        bool jumpPressed = kb.upArrowKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame;
        if (grounded && jumpPressed)
        {
            yVelocity = jumpSpeed;
            grounded = false;
        }

        // 3. Roll: stay low for as long as the key is held (only on the ground)
        bool slideHeld = kb.downArrowKey.isPressed || kb.sKey.isPressed;
        bool sliding = grounded && slideHeld;
        isSliding = sliding;

        // 4. Collider size: only the collider changes, the root stays at scale 1
        float height = sliding ? slideHeight : standHeight;
        float halfHeight = height / 2f;
        if (capsule != null)
        {
            capsule.height = height;
            capsule.center = Vector3.zero;
        }

        // The model container keeps its feet on the ground (the animator adds the roll on top)
        if (visual != null)
        {
            visual.localPosition = new Vector3(0f, -halfHeight, 0f);
            visual.localRotation = Quaternion.identity;
            visual.localScale = Vector3.one;
        }

        // 5. Move forward and glide towards the middle of the chosen lane
        transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, lane * laneWidth, laneSwitchSpeed * Time.deltaTime);

        // 6. Keep the feet where they were when the body changes size
        pos.y -= previousHalfHeight - halfHeight;
        previousHalfHeight = halfHeight;

        // 7. Gravity
        yVelocity += gravity * Time.deltaTime;
        pos.y += yVelocity * Time.deltaTime;

        // 8. Raycast ground check: shoot a ray down from the body's center
        grounded = false;
        RaycastHit hit;
        if (yVelocity <= 0f && Physics.Raycast(pos, Vector3.down, out hit, halfHeight + groundCheckExtra, ~0, QueryTriggerInteraction.Ignore))
        {
            pos.y = hit.point.y + halfHeight;
            yVelocity = 0f;
            grounded = true;
        }

        transform.position = pos;
    }
}