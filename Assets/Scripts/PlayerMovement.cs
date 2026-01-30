using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // REFERENCES
    public Rigidbody2D rb;
    public Animator animator;
    public PlayerHealth playerHealth;

    // MOVEMENT
    public float x;
    [SerializeField] private float speed = 1.25f;

    // GROUND CHECK
    public LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundBoxSize = new Vector2(0.075f, 0.025f);

    public bool grounded;
    private bool wasGrounded;
    public bool landedThisFrame;

    // JUMP SHARED
    public float coyoteCounter;
    [SerializeField] private float coyoteTime = 0.1f;
    public bool jumpCutApplied;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>(); 
    }

    public void MoveHandler()
    {
        x = Input.GetAxisRaw("Horizontal");
        animator.SetBool("running", x != 0f);
    }

    public void Movement()
    {
        if (playerHealth.IsKnockedBack) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        else rb.velocity = new Vector2(x * speed, rb.velocity.y);

        if (x < 0) transform.localScale = new Vector3(-1f, 1f, 1f);
        else if (x > 0) transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void CheckGrounded()
    {
        bool touchingGround = Physics2D.OverlapBox(groundCheck.position, groundBoxSize, 0f, groundLayer);

        grounded = touchingGround && rb.velocity.y <= 0.01f;
        landedThisFrame = !wasGrounded && grounded && rb.velocity.y <= 0f;
        wasGrounded = grounded;

        if (grounded)
        {
            coyoteCounter = coyoteTime;
            jumpCutApplied = false;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);
    }
}
