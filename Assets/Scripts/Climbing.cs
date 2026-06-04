using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public PlayerController pm;
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask Wall;

    [Header("Climbing Settings")]
    public float climbSpeed = 5f;
    public float maxClimbTime = 2f;
    private float climbTimer;

    [Header("Detection")]
    public float detectionLength = 0.7f;  // Increase this if it feels picky
    public float sphereCastRadius = 0.3f; // Forgiving bubble size
    public float maxWallLookAngle = 45f;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private void Awake()
    {
        climbTimer = maxClimbTime;
    }

    private void Update()
    {
        WallCheck();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.climbing) ClimbingMovement();
    }

    private void WallCheck()
    {
        // Use orientation forward but flattened to prevent look-angle pitch issues
        Vector3 projectForward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up).normalized;

        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, projectForward,
                        out frontWallHit, detectionLength, Wall);

        wallLookAngle = wallFront
            ? Vector3.Angle(projectForward, -frontWallHit.normal)
            : 0f;

        pm.climbingPossible = wallFront && wallLookAngle < maxWallLookAngle;

        if (pm.isGrounded)
            climbTimer = maxClimbTime;
    }

    private void StateMachine()
    {
        if (pm.wallrunning)
        {
            if (pm.climbing) StopClimbing();
            return;
        }

        // Changed from Input.GetKey(KeyCode.W) to use your PlayerController's moveY input system value!
        if (wallFront && pm.moveY > 0.1f && wallLookAngle < maxWallLookAngle)
        {
            if (!pm.climbing && climbTimer > 0) StartClimbing();
            
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer <= 0) StopClimbing();
        }
        else
        {
            if (pm.climbing) StopClimbing();
        }
    }

    private void StartClimbing()
    {
        pm.climbing = true;
        rb.useGravity = false;
    }

    private void StopClimbing()
    {
        pm.climbing = false;
        rb.useGravity = true;
    }

    private void ClimbingMovement()
    {
        // Keep horizontal momentum, but directly drive vertical speed based on input
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, climbSpeed * pm.moveY, rb.linearVelocity.z);
    }
}