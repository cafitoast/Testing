using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float moveX;
    private float moveY;
    public float speed = 5f;
    public float jumpForce = 8f;
    public float airMultiplier = 0.8f; 
    public float dashForce = 5f;
    public float dashCooldown = 0f;
    public Transform groundCheck;        
    public float groundDistance = 0.2f; 
    public LayerMask groundMask;         
    private bool isGrounded;
    public Transform orientation; 

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
        if(dashCooldown >= 0)
        {
            Vector3 dashDirection = orientation.forward * 300;
            if(dashDirection == Vector3.zero)
            {
                dashDirection = orientation.forward;
            }
            rb.AddForce(dashDirection);
        }
    }
    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 movement = orientation.forward * moveY + orientation.right * moveX;

        if (isGrounded)
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + movement * speed * airMultiplier * Time.fixedDeltaTime);
        }
    }
}
