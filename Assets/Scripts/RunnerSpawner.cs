using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class RunnerSpawner : MonoBehaviour
{
    [Header("Prefab & spawn time")]
    public GameObject enemyPrefab;
    public GameObject exposionPrefab;
    Animator animator;
    public float spawnInterval = 5f;

    [Header("Spawn Points")]
    public Transform spawnPoint;

    [Header("Player detection")]
    public GameObject player;
    public float spawningDistance = 1.6f;

    [Header("Enemy settings")]
    [SerializeField] private float moveDirection = -1f;

    [Header("Spawner Health")]
    public int maxHealth = 10;
    private int currentHealth;
    public int health;

    private Coroutine spawnCoroutine;

    AudioSource audioSource;
    public AudioClip hitSound; //sonido de daño

    PlayerMov playerScript;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        health = currentHealth;
        animator = GetComponent<Animator>();
        playerScript = player.GetComponent<PlayerMov>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(playerScript.isDead && spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        if(player == null) return;
        if(currentHealth <= 0) return;
        if(playerScript.isDead) return;

        float distance = Math.Abs(player.transform.position.x - transform.position.x);
        if (distance <= spawningDistance && spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemies());
        }
        else if (distance > spawningDistance && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        RunnerEnemy enemyScript = newEnemy.GetComponent<RunnerEnemy>();
        enemyScript.moveDirection = moveDirection;
    }
    public void Hit(int damage)
    {
        if (currentHealth <= 0) return;
        audioSource.PlayOneShot(hitSound, 0.5f);
        currentHealth -= damage;
        health = currentHealth;
        animator.SetTrigger("Hit");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        GetComponent<Collider2D>().enabled = false;
        Instantiate(exposionPrefab, transform.position, Quaternion.identity);
        animator.SetTrigger("Destroyed");
        
    }

}
