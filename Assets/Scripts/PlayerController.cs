using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Climbing climbingScript;
    private Rigidbody rb;
    private float moveX;
    private float moveY;
    private bool isSprinting;
    public float baseSpeed = 5f;
    public float sprintBonus = 5f;
    private float currentSpeed;  
    public float jumpForce = 8f;
    public float airMultiplier = 5f; 
    public float dashForce = 5f;
    public float dash = 1.0f;
    private float NextDashTime = 2.0f;
    public Transform groundCheck;        
    public float groundDistance = 0.2f; 
    public LayerMask groundMask;         
    public bool isGrounded;
    public Transform orientation; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = baseSpeed;
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

        rb.AddForce(dashDirection * 15, ForceMode.Impulse); // ForceMode.Impulse recommended here too
    }
}
    void OnSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift)) // while player holds shift he can sprint
        {
            if(!isSprinting) 
                {
                currentSpeed += sprintBonus;
                isSprinting = true; // right after we apply the double speed or whatever, we set the bool to true so it can't do it over and over again.
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) // as soon as he lets go, the bool turns false and the speed is reset
        {
            currentSpeed = baseSpeed;
            isSprinting = false;
        }
    }
void FixedUpdate()
{
    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    // Don't fight the climbing script
    if (climbingScript != null && climbingScript.climbing) return;

    Vector3 movement = orientation.forward * moveY + orientation.right * moveX;

    if (isGrounded)
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    else
        rb.MovePosition(rb.position + movement * currentSpeed * airMultiplier * Time.fixedDeltaTime);
}   
}