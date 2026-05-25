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


    private bool climbing;


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
    if (climbing) ClimbingMovement();
    
}


    private void StateMachine()
    {

        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle)
        {
            if (!climbing && climbTimer > 0) StartClimbing();
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer <= 0) StopClimbing();
        }
        else
        {
            if (climbing) StopClimbing();
        }
    }


private void WallCheck()
{
    wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward,
                    out frontWallHit, detectionLength, Wall);

    wallLookAngle = wallFront
        ? Vector3.Angle(orientation.forward, -frontWallHit.normal)
        : 0f;

    if (pm.isGrounded)
        climbTimer = maxClimbTime;
}


private void StartClimbing()
{
    climbing = true;
    rb.useGravity = false;
}

private void StopClimbing()
{
    climbing = false;
    rb.useGravity = true;
}


    private void ClimbingMovement()
{
    rb.linearVelocity = new Vector3(rb.linearVelocity.x, climbSpeed, rb.linearVelocity.z);
}

}

