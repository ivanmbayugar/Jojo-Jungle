using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private float invulnerabilityTime = 2f;

    public bool isDead = false;
    private bool isInvulnerable = false;
    public bool IsKnockedBack => isKnockedBack; 

    // KNOCKBACK
    private bool isKnockedBack = false;
    [SerializeField] private float knockbackDuration = 0.15f;
    [SerializeField] private float knockbackUpSpeed = 3f;
    private float knockbackTimer;

    // REFERENCES
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private GameManager gameManager;

    [SerializeField] private Collider2D damageCol;

    public AudioClip hurtSound;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Hit()
    {
        if (isInvulnerable || isDead) return;

        audioSource.PlayOneShot(hurtSound, 0.25f);
        animator.SetTrigger("Hit");

        health--;
        if (health <= 0) Death();
        else StartCoroutine(InvulnerabilityCoroutine());
    }

    void Death()
    {
        isDead = true;
        gameManager.CheckPlayerDeath();
        damageCol.enabled = false;

        rb.velocity = Vector2.zero;
        animator.SetTrigger("Dead");
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        damageCol.enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float timer = 0f;

        while (timer < invulnerabilityTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        sr.enabled = true;
        damageCol.enabled = true;
        isInvulnerable = false;
    }

    public void CheckKnockBack()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f) isKnockedBack = false;
        }
    }

    void ApplyKnockback()
    {
        if (isDead || isKnockedBack) return;

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        rb.velocity = new Vector2(rb.velocity.x, knockbackUpSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            Hit();

        if (collision.gameObject.CompareTag("Trap"))
        {
            Hit();
            ApplyKnockback();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        Hit();
    }
}
