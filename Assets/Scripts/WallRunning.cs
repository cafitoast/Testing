
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    public LayerMask Wall;
    public LayerMask Ground;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Wall Jump")]
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 12f;

    [Header("Exit Momentum")]
    public float exitMomentumMultiplier = 1.5f;

    public Transform orientation;
    private PlayerController pm;
    private Rigidbody rb;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerController>();
    }

    void Update()
    {
        CheckForWall();
        StateMachine();
    }

    void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, Wall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, Wall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, Ground);
    }

    private void StateMachine()
    {
        float verticalInput = pm.moveY;

        if (pm.climbing)
        {
            if (pm.wallrunning) StopWallRun();
            return;
        }

        if (pm.climbingPossible)
        {
            if (pm.wallrunning) StopWallRun();
            return;
        }

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!pm.wallrunning)
                StartWallRun();

            
            if (Input.GetKeyDown(KeyCode.Space))
                WallJump();
        }
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

  private void StartWallRun()
{
    pm.wallrunning = true;
    rb.useGravity = false;
    wallRunTimer = maxWallRunTime;
    Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
    Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

    if ((orientation.forward - wallForward).magnitude >
        (orientation.forward - -wallForward).magnitude)
        wallForward = -wallForward;

    float currentSpeed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
    float entrySpeed = Mathf.Max(currentSpeed, pm.wallrunSpeed); // don't slow the player down
    rb.linearVelocity = new Vector3(
        wallForward.x * entrySpeed,
        rb.linearVelocity.y,
        wallForward.z * entrySpeed
    );
}
private void WallRunningMovement()
{
    wallRunTimer -= Time.fixedDeltaTime;
    if (wallRunTimer <= 0) { StopWallRun(); return; }

    // Smoothly ramp down vertical velocity instead of snapping to -2
    float targetY = -2f;
    float newY = Mathf.Lerp(rb.linearVelocity.y, targetY, Time.fixedDeltaTime * 10f);

    Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
    Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

    if ((orientation.forward - wallForward).magnitude >
        (orientation.forward - -wallForward).magnitude)
        wallForward = -wallForward;

    Vector3 targetVelocity = wallForward * pm.wallrunSpeed;
    Vector3 currentHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    Vector3 blended = Vector3.Lerp(currentHorizontal, targetVelocity, Time.fixedDeltaTime * 8f);

    rb.linearVelocity = new Vector3(blended.x, newY, blended.z);
}

    private void WallJump()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 jumpDirection = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Zero out vertical so the upward force is consistent
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(jumpDirection, ForceMode.Impulse);

        StopWallRun();
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        rb.useGravity = true;

        // Preserve and boost momentum in the wall-forward direction
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude >
            (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        rb.AddForce(wallForward * speed * exitMomentumMultiplier, ForceMode.Impulse);
    }
}