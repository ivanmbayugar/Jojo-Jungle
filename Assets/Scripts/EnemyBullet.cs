using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public AudioClip shootSound;
    public float speed = 10f;
    private bool touching = false;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Camera.main.GetComponent<AudioSource>().PlayOneShot(shootSound, 0.35f);
    }

    // Update is called once per frame
    void Update()
    { 
        if (touching) return;
        rb.velocity = Vector2.right * direction.x * speed;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        if (dir.x < 0) transform.localScale = new Vector3(-1, 1, 1); else transform.localScale = new Vector3(1, 1, 1);
    }
    public void TouchingSmth()
    {
        rb.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        touching = true;

        animator.SetTrigger("touching");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (touching) return;

        PlayerMov player = collision.GetComponentInParent<PlayerMov>();
        if (player != null)
        {
            TouchingSmth();
        }

        if(collision.CompareTag("Wall")) TouchingSmth();

        if(collision.CompareTag("PlayerBullet")) TouchingSmth();

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) TouchingSmth();
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
