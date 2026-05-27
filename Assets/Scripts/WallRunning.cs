using System;
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

        // Climbing takes priority — if actively climbing, stop wall run and yield
        if (pm.climbing)
        {
            if (pm.wallrunning) StopWallRun();
            return;
        }

        // Wall directly ahead and player is eligible to climb — yield to Climbing script
        if (pm.climbingPossible)
        {
            if (pm.wallrunning) StopWallRun();
            return;
        }

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!pm.wallrunning)
                StartWallRun();
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
    }

    private void WallRunningMovement()
    {
        wallRunTimer -= Time.fixedDeltaTime;
        if (wallRunTimer <= 0)
        {
            StopWallRun();
            return;
        }

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude >
            (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        rb.useGravity = true;
    }
}