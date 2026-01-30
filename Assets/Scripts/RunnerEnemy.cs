using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class RunnerEnemy : MonoBehaviour
{
    PlayerMov playerscript;
    public GameObject player;
    public float speed = 3.5f;
    public float moveDirection = 1f;
    private Animator animator;
    private int health = 1;

    //Audios
    private AudioSource audioSource;
    public AudioClip deathSound; //sonido de muerte
    public AudioClip hurtSound; //sonido de daño

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerscript = FindObjectOfType<PlayerMov>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0) return;
        if (player == null) return;

        Movement();
    }
    void Movement()
    {
        if (player == null) return;
        if(health <= 0) return;

        if(moveDirection > 0) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);

        Vector3 move = new Vector3(moveDirection * speed * Time.deltaTime, 0, 0);
        transform.position += move;
    }
        public void Hit()
    {
        audioSource.PlayOneShot(hurtSound, 0.25f);
        health--;
        if (health <= 0) Death();
    }

    void Death()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        audioSource.PlayOneShot(deathSound, 0.15f);
        animator.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(playerscript.isDead) Physics2D.IgnoreLayerCollision(8, 7, true); //ignora colisiones entre player y enemigos
        if (collision.gameObject.CompareTag("Player")) Hit();  
        if (collision.gameObject.CompareTag("Trap")) Hit();
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
