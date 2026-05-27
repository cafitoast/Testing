using UnityEngine;

public class Climbing : MonoBehaviour
{
    // References
    public PlayerController pm;
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask Wall;

    // Climbing
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    // Detection
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    public float wallPressForce = 50f;

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
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward,
                        out frontWallHit, detectionLength, Wall);

        wallLookAngle = wallFront
            ? Vector3.Angle(orientation.forward, -frontWallHit.normal)
            : 0f;

        // Tell PlayerController whether climbing is geometrically possible this frame.
        // WallRunning reads this to yield before it even tries to start.
        pm.climbingPossible = wallFront && wallLookAngle < maxWallLookAngle;

        // Reset climb stamina when grounded
        if (pm.isGrounded)
            climbTimer = maxClimbTime;
    }

    private void StateMachine()
    {
        // Wall-running takes priority over climbing only if already in that state
        // and no climbable wall is directly ahead — handled by WallRunning yielding
        // via pm.climbingPossible, so no extra guard needed here.

        // Don't start or continue climbing while wall-running
        if (pm.wallrunning)
        {
            if (pm.climbing) StopClimbing();
            return;
        }

        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle)
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
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, climbSpeed, rb.linearVelocity.z);
    }
}