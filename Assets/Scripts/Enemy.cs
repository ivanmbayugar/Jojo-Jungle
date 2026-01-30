using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    PlayerMov playerscript;
    PlayerHealth playerHealth;
    public GameObject bullet;
    public GameObject player;
    public GameObject weapon;
    private Animator animator;
    public float cooldown = 0.75f;
    private float lastShoot;
    private int health = 3;

    //Audios
    private AudioSource audioSource;
    public AudioClip deathSound; //sonido de muerte
    public AudioClip hurtSound; //sonido de daño

    void Start()
    {
        animator = GetComponent<Animator>();
        playerscript = player.GetComponent<PlayerMov>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;
        if (playerscript.isDead) return;

        //if(newPlayer == null) return;
        //if(playerHealth.isDead) return;

        CheckDirection();
        ShootHandler();
    }
    void CheckDirection()
    {
        if (player == null) return;

        Vector3 direction = player.transform.position - transform.position;
        if (direction.x > 0)transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);

        /*if (newPlayer == null) return;

        Vector3 direction = newPlayer.transform.position - transform.position;
        if (direction.x > 0)transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);*/
    }
    void ShootHandler()
    {
        float distance = Math.Abs(player.transform.position.x - transform.position.x);
        //float distance = Math.Abs(newPlayer.transform.position.x - transform.position.x);

        if (distance < 1.5f && Time.time > lastShoot + cooldown)
        {
            Shoot();
            lastShoot = Time.time;
        }   
    }

    void Shoot()
    {
        if (health <= 0) return;
        animator.SetTrigger("Shoot");
        Vector3 direction;
        if (transform.localScale.x > 0)direction = Vector3.right;
        else direction = Vector3.left;

        GameObject bulletScript = Instantiate(bullet, weapon.transform.position, Quaternion.identity);
        bulletScript.GetComponent<EnemyBullet>().SetDirection(direction);
    }
    public void Hit()
    {
        audioSource.PlayOneShot(hurtSound, 0.15f);
        animator.SetTrigger("Hit");
        health--;
        if (health <= 0) Death();
    }

    void Death()
    {
        audioSource.PlayOneShot(deathSound, 0.25f);
        animator.SetTrigger("Death");
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }

}
