using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    //================================= REFERENCES ==========================================
    private Rigidbody2D rb; //rigidbody del player
    private Animator animator; //animator del player
    [SerializeField] private GameManager gameManager; //referencia a gameManager
    [SerializeField] private LayerMask groundLayer; //capa del suelo
    [SerializeField] private LayerMask dropLayer; //capa atravesable 
    [SerializeField] private Transform groundCheck; //punto de comprobacion del suelo
    [SerializeField] private GameObject bullet; //prefab de la bala
    [SerializeField] private GameObject weapon; //punto de disparo

    //================================= MOVEMENT ==========================================
    private float x; //eje horizontal del player
    [SerializeField] private float speed = 1.25f; //velocidad del player
    [SerializeField] private Vector2 groundBoxSize = new Vector2(0.075f, 0.025f);

    //================================= JUMP ============================================
    [SerializeField] private float coyoteTime = 0.1f; //tiempo de coyote
    [SerializeField] private float jumpCutter = 0.5f; //reductor de salto
    private float coyoteCounter; //contador del tiempo de coyote
    private bool jumpCutApplied; //si se ha aplicado el reductor de salto
    [SerializeField] private float jumpForce = 3.5f; //fuerza del salto
    [SerializeField] private float dropThroughTime = 0.25f;
    private bool isDropping;

    //================================= COMBAT ============================================
    private float lastShoot; //tiempo del ultimo disparo
    [SerializeField] private float cooldown = 0.25f; //tiempo entre disparos
    [SerializeField] private int health = 3; //vida del player

    //================================= STATES ============================================
    private bool isInvulnerable = false; //si el player es invulnerable
    [SerializeField] private float invulnerabilityTime = 2f; //duracion de la invulnerabilidad
    public bool isDead = false; //si el player está muerto
    public bool grounded; //si el player esta en el suelo

    //================================= KNOCKBACK ==========================================
    private bool isKnockedBack = false; //si el player esta siendo retrocedido
    [SerializeField] private float knockbackDuration = 0.15f; //duracion del retroceso
    [SerializeField] private float knockbackUpSpeed = 3f; //fuerza del retroceso
    private float knockbackTimer = 0f; //contador del retroceso

    //================================= AUDIO ==========================================
    private AudioSource audioSource;
    public AudioClip hurtSound; //sonido de daño
    public AudioClip jumpSound; //sonido de salto

    //================================= COLLIDERS ==========================================
    [Header("Colliders")]
    [SerializeField] private Collider2D groundCol; 
    [SerializeField] private Collider2D damageCol;


    //================================= UNITY ==========================================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update() //maneja el input del player
    {
        if (isDead) return;

        CheckGrounded();
        MoveHandler();
        ShootHandler();
        JumpInput();
        CheckKnockBack();
        
    }
    private void FixedUpdate() //maneja el movimiento del player
    {
        if (isDead) return;

        Movement();
    }

    //================================= INPUT ==========================================
    void JumpInput() //maneja el input de salto
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && grounded)
        {
            StartCoroutine(DropThroughPlatform());
            return;
        } 

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && coyoteCounter > 0f)
        {
            Jump();
            coyoteCounter = 0f;
        }

        if((Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W)) && rb.velocity.y > 0f && !jumpCutApplied)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutter);
            jumpCutApplied = true;
        }
    }
        void MoveHandler() //maneja el movimiento horizontal
    {
        x = Input.GetAxisRaw("Horizontal");
        animator.SetBool("running", x != 0f);
    }
    
    //================================= MOVEMENT ==========================================
    void Movement() //aplica la velocidad al rigidbody
    {
        if (isKnockedBack && rb.velocity.y < knockbackUpSpeed) rb.velocity = new Vector2(x * speed, knockbackUpSpeed);
        else rb.velocity = new Vector2(x * speed, rb.velocity.y);
        if (0 > x) transform.localScale = new Vector3(-1f, 1f, 1f);
        else if (x > 0) transform.localScale = new Vector3(1f, 1f, 1f);
    }

    //================================= JUMP ==========================================
    void CheckGrounded() //comprueba si el player esta en el suelo
    {
        grounded = Physics2D.OverlapBox(groundCheck.position, groundBoxSize, 0f, groundLayer);

        if (grounded)
        {
            coyoteCounter = coyoteTime;
            jumpCutApplied = false;
        } 
        else coyoteCounter -= Time.deltaTime;
    }
    void Jump() //aplica la fuerza de salto
    {
        audioSource.PlayOneShot(jumpSound, 0.05f);
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    private IEnumerator DropThroughPlatform()
    {
        if (isDropping) yield break; 
        isDropping = true;

        // Raycast corto para detectar la plataforma debajo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, dropLayer);

        if (hit && hit.collider != null)
        {
            // Ignora colisión SOLO entre este player y esta plataforma
            Physics2D.IgnoreCollision(groundCol, hit.collider, true);

            yield return new WaitForSeconds(dropThroughTime);

            Physics2D.IgnoreCollision(groundCol, hit.collider, false);
        }

        isDropping = false;
    }

    //================================= SHOOT ==========================================
    void ShootHandler() //maneja el disparo con cooldown
    {
        if ((Input.GetKey(KeyCode.Z) || Input.GetMouseButton(0)) && Time.time >  lastShoot + cooldown)
        {
            Shoot();
            lastShoot = Time.time;
        }        
    }
    void Shoot() //instancia la bala y le da direccion
    {
        animator.SetTrigger("shooting");
        Vector3 direction;
        if (transform.localScale.x > 0)direction = Vector3.right;
        else direction = Vector3.left;

        GameObject bulletScript = Instantiate(bullet, weapon.transform.position, Quaternion.identity);
        bulletScript.GetComponent<Bullet>().SetDirection(direction);
    }

    //================================= DAMAGE ==========================================
    public void Hit() //reduce la vida del player al ser golpeado
    {
        if (isInvulnerable) return;
        if (isDead) return;

        audioSource.PlayOneShot(hurtSound, 0.25f);
        animator.SetTrigger("Hit");
        health--;
        if (health <= 0) Death();
        else StartCoroutine(InvulnerabilityCoroutine());
    }
    
    void Death() //animacion de muerte
    {
        isDead = true;
        gameManager.CheckPlayerDeath();
        damageCol.enabled = false;

        rb.velocity = Vector2.zero;
        animator.SetTrigger("Dead");
    }
    private IEnumerator InvulnerabilityCoroutine() //hace al player invulnerable por un tiempo
    {
        isInvulnerable = true;
        damageCol.enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float timer = 0f;

        while (timer < invulnerabilityTime)
        {
            sr.enabled = !sr.enabled; //parpadeo visual
            yield return new WaitForSeconds(0.1f); //ajusta la velocidad del parpadeo
            timer += 0.1f;
        }

        sr.enabled = true; //asegura que el sprite este visible al final
        damageCol.enabled = true;
        isInvulnerable = false;
    }

    //================================= KNOCKBACK ==========================================
    void CheckKnockBack() //verifica y maneja el retroceso
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f) isKnockedBack = false;
        }
    }
    void ApplyKnockback() //aplica retroceso al player al ser golpeado por una trampa
    {
        if (isDead) return;
        if (isKnockedBack) return;

        isKnockedBack = true;
        knockbackTimer = knockbackDuration; 
        rb.velocity = new Vector2(rb.velocity.x, knockbackUpSpeed); // Knockback solo vertical
    }

    //================================= COLLISIONS ==========================================
    void OnCollisionEnter2D(Collision2D collision)
    {           
            if (collision.gameObject.CompareTag("Trap"))
        {
            Hit();
            ApplyKnockback();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("EnemyBullet")) Hit();

        if (other.CompareTag("Enemy"))
        {
            Hit();
        } 
    }
    

    //================================= GIZMOS ==========================================
    void OnDrawGizmosSelected() //dibuja el radio de comprobacion del suelo
    {
        if (groundCheck == null) return;

        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);

    }

}