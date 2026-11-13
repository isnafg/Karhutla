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

    int lane = 0;
    float yVelocity = 0f;
    bool grounded = false;
    float previousHalfHeight = 1f;
    float startSpeed;
    float runTime = 0f;

    void Start()
    {
        startSpeed = runSpeed;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;

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

        // 3. Slide: stay low for as long as the key is held (only on the ground)
        bool slideHeld = kb.downArrowKey.isPressed || kb.sKey.isPressed;
        bool sliding = grounded && slideHeld;

        // 4. Body size (the capsule is 2 tall at scale 1)
        float height = sliding ? slideHeight : standHeight;
        float halfHeight = height / 2f;
        transform.localScale = new Vector3(1f, halfHeight, 1f);

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