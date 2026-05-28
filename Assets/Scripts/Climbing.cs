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