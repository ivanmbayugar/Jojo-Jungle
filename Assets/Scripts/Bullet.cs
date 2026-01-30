using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public AudioClip shootSound;

    public float speed = 10f;
    private bool touching = false;
    private Rigidbody2D rb;
    private Vector2 direction;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Camera.main.GetComponent<AudioSource>().PlayOneShot(shootSound, 0.35f);
        animator = GetComponent<Animator>();
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

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (touching) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Hit();
            TouchingSmth();
        }

        if (collision.CompareTag("Wall"))
        {
            TouchingSmth();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            TouchingSmth();
        }

        RunnerEnemy runnerEnemy = collision.GetComponent<RunnerEnemy>();
        if (runnerEnemy != null)
        {
            runnerEnemy.Hit();
            TouchingSmth();
        }

        EnemyBullet enemyBullet = collision.GetComponent<EnemyBullet>();
        if (enemyBullet != null)
        {
            TouchingSmth();
        }

        RunnerSpawner runnerSpawner = collision.GetComponent<RunnerSpawner>();
        if (runnerSpawner != null && runnerSpawner.health > 0)
        {
            runnerSpawner.Hit(1);
            TouchingSmth();
        }

    }
}
