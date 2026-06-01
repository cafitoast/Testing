using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement")]
    public float moveX;
    public float moveY;
    public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float wallrunSpeed;

    [Header("Jumping")]
    public float jumpForce = 8f;
    public float airMultiplier = 5f;

    [Header("Dashing")]
    public float dashForce = 5f;
    public float dash = 1.0f;
    private float NextDashTime = 0f;

    [Header("Drag")]
    public float groundDrag = 5f;    // snappy ground stops
    public float airDrag = 0.5f;     // minimal air resistance, preserves momentum
    public float wallrunDrag = 0f;   // no drag during wallrun, momentum is king
    public float grapplingDrag = 0f; // no drag while swinging

    [Header("Checking")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public bool isGrounded;
    public Transform orientation;

    public MovementState state;
    public TextMeshProUGUI timerText;

    private float secondsCount;
    private int minuteCount;
    private int hourCount;

    public enum MovementState
    {
        freeze,
        walking,
        sprinting,
        wallrunning,
        air
    }

    public bool wallrunning;
    public bool climbing;
    public bool climbingPossible;
    public bool freeze;
    public bool activeGrapple;

    void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.linearVelocity = Vector3.zero;
            rb.linearDamping = groundDrag;
        }
        else if (activeGrapple)
        {
            // Let the spring joint do its work unimpeded
            rb.linearDamping = grapplingDrag;
        }
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallrunSpeed;
            rb.linearDamping = wallrunDrag;
        }
        else if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            rb.linearDamping = groundDrag;
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            rb.linearDamping = groundDrag;
        }
        else
        {
            state = MovementState.air;
            rb.linearDamping = airDrag;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        moveX = moveInput.x;
        moveY = moveInput.y;
    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        }
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    void OnDash()
    {
        if (Time.time > NextDashTime)
        {
            NextDashTime = dash + Time.time;

            Vector3 dashDirection = orientation.forward;
            if (dashDirection == Vector3.zero)
                dashDirection = transform.forward;

            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        }
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        rb.linearVelocity = velocityToSet;
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
        Invoke(nameof(ResetRestrictions), 3f);
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private void Update()
    {
        StateHandler();
        UpdateTimerUI();
    }

    public void UpdateTimerUI()
    {
        secondsCount += Time.deltaTime;
        timerText.text = hourCount + "h:" + minuteCount + "m:" + (int)secondsCount + "s";

        if (secondsCount >= 60)
        {
            minuteCount++;
            secondsCount = 0;
        }
        if (minuteCount >= 60)
        {
            hourCount++;
            minuteCount = 0;
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (climbing) return;
        if (wallrunning) return;
        if (activeGrapple) return;

        Vector3 movement = orientation.forward * moveY + orientation.right * moveX;
        if (isGrounded)
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        else
            rb.MovePosition(rb.position + movement * moveSpeed * airMultiplier * Time.fixedDeltaTime);
    }
}