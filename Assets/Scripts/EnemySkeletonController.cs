using System.Collections;
using UnityEngine;

public class EnemySkeletonController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 6;
    private int currentHealth;

    private SpriteRenderer spriteRenderer;
    private bool isDead;

    [Header("Attack")]
    [SerializeField] private float attackRange = 0.6f; // ajusta según tamaño del sprite

    [SerializeField] private float attackCooldown = 1.5f;

    private float attackTimer;
    private Transform player;

    [SerializeField] private float attackMeleeRange = 0.6f; // solo melee



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }



    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    // SOLO daño de espada
    public void TakeSwordDamageFromPosition(int damage, Vector3 swordPos)
    {
        if (isDead) return;

        // si la espada viene de delante → no daño
        if (IsFacingSword(swordPos))
            return;

        currentHealth -= damage;
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }


    public bool IsFacingSword(Vector3 swordPos)
    {
        float dirToSword = swordPos.x - transform.position.x;
        float facing = transform.localScale.x;

        // si la espada viene desde donde está mirando, es frente (escudo)
        return Mathf.Sign(dirToSword) == Mathf.Sign(facing);
    }

    IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        // Animación de muerte
        GetComponent<Animator>().SetTrigger("Death");
    }

    void Update()
    {
        if (isDead || player == null) return;

        MirarAlJugador();
        ManejarAtaque();
    }


    [SerializeField] private float tiempoReaccionGiro = 0.1f; 

    private float timerGiro;

    void MirarAlJugador()
    {
        float dist = Mathf.Abs(player.position.x - transform.position.x);
        if (dist <= attackMeleeRange && attackTimer <= 0f)
        {
            AttackPlayer();
            attackTimer = attackCooldown;
        }


        timerGiro -= Time.deltaTime;
        if (timerGiro > 0f) return;

        float dir = player.position.x - transform.position.x;
        if (dir > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        timerGiro = tiempoReaccionGiro;
    }





    void ManejarAtaque()
    {
        attackTimer -= Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && attackTimer <= 0f)
        {
            AttackPlayer();
            attackTimer = attackCooldown;
        }
    }



    void AttackPlayer()
    {
        GetComponent<Animator>().SetTrigger("Attack");
    }

    public void DoAttackDamage()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > attackRange) return; // si ya se alejó, no hay daño

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(1, Vector2.zero);
        }
    }




}
