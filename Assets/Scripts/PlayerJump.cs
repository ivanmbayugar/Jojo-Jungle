using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerMovement movement;
    public AudioSource audioSource;

    [SerializeField] private float jumpForce = 2.5f;
    [SerializeField] private float jumpCutter = 0.4f;

    public bool wannaJump;

    [SerializeField] private float dropThroughTime = 0.175f;
    private bool isDropping;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        movement = GetComponent<PlayerMovement>();
    }

    public AudioClip jumpSound;

    public void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) wannaJump = true;
        if (Input.GetKeyUp(KeyCode.Space)) wannaJump = false;

        if (Input.GetKey(KeyCode.DownArrow) && wannaJump && movement.grounded)
        {
            StartCoroutine(DropThroughPlatform());
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && movement.coyoteCounter > 0f)
        {
            Jump();
            movement.coyoteCounter = 0f;
        }

        if (wannaJump && movement.landedThisFrame)
        {
            Jump();
        }

        if (!wannaJump && rb.velocity.y > 0f && !movement.jumpCutApplied)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutter);
            movement.jumpCutApplied = true;
        }
    }

    void Jump()
    {
        audioSource.PlayOneShot(jumpSound, 0.05f);
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private IEnumerator DropThroughPlatform()
    {
        if (isDropping) yield break;
        isDropping = true;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, movement.groundLayer);

        if (hit)
        {
            PlatformEffector2D effector = hit.collider.GetComponent<PlatformEffector2D>();
            if (effector != null)
            {
                effector.rotationalOffset = 180f;
                yield return new WaitForSeconds(dropThroughTime);
                effector.rotationalOffset = 0f;
            }
        }

        isDropping = false;
    }
}
