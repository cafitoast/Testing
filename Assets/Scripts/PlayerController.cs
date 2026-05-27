using System;
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
    private float NextDashTime = 2.0f;
    
    [Header("Checking")] 
    public Transform groundCheck;        
    public float groundDistance = 0.2f; 
    public LayerMask groundMask;         
    public bool isGrounded;
    public Transform orientation; 

    public MovementState state; 
    public enum MovementState
    {
        walking, 
        sprinting,
        wallrunning,
        air
    }
    public bool wallrunning; 
    public bool climbing;
    public bool climbingPossible;
    void StateHandler()
    {
        //wall running
        if (wallrunning)
        {
           state = MovementState.wallrunning; 
           moveSpeed = wallrunSpeed; 
        }
        //sprinting 
        else if(isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
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
void OnDash()
{
    if (dash >= 0 && Time.time > NextDashTime)
    {
        NextDashTime = dash + Time.time;

        Vector3 dashDirection = orientation.forward;

        if (dashDirection == Vector3.zero)
            dashDirection = transform.forward; // safe fallback

        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse); // ForceMode.Impulse recommended here too
    }
}
    
private void Update()
    {
        StateHandler();
    }
      
void FixedUpdate()
{
    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    
    if (climbing) return;
    if (wallrunning) return;

    Vector3 movement = orientation.forward * moveY + orientation.right * moveX;
        if (isGrounded)
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        else
            rb.MovePosition(rb.position + movement * moveSpeed * airMultiplier * Time.fixedDeltaTime);
        }   
    }