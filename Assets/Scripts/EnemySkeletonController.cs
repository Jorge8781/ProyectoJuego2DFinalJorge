using UnityEngine;
using System.Collections;

public class EnemySkeletonController : MonoBehaviour
{
    [Header("Movement")]
    public float chaseSpeed = 2.5f;
    public float detectionRadius = 5f;
    public LayerMask playerLayer;

    [Header("Attack")]
    public int damage = 1;
    public float attackCooldown = 1.2f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool chasing;
    private bool isAttacking;
    private bool isDead;
    private float attackTimer;

    [Header("Distances")]
    public float stopDistance = 0.6f;

    [Header("References")]
    public Transform attackPivot;

    [Header("Turning")]
    public float turnDelay = 0.4f;
    private bool canTurn = true;

    [Header("Turning")]

    private bool isTurning = false;
    private float currentFacing = 1f; // 1 = derecha, -1 = izquierda

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

    }

    void Update()
    {
        DetectPlayer();
        attackTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        Move();
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            detectionRadius,
            playerLayer
        );

        if (hit)
        {
            player = hit.transform;
            chasing = true;
        }
        else
        {
            chasing = false;
            player = null;
        }
    }

    void Move()
    {
        if (isDead || isAttacking || !chasing || player == null)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("Walking", false);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stopDistance)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("Walking", false);
            return;
        }

        Vector2 dir = (player.position - transform.position).normalized;

        rb.velocity = new Vector2(dir.x * chaseSpeed, 0);
        animator.SetBool("Walking", true);

        // 🔥 SOLO pedir giro, no hacerlo
        if (!isTurning && Mathf.Sign(dir.x) != currentFacing)
        {
            StartCoroutine(TurnWithDelay(dir.x));
        }
    }


    IEnumerator TurnWithDelay(float dirX)
    {
        isTurning = true;

        yield return new WaitForSeconds(turnDelay);

        currentFacing = Mathf.Sign(dirX);

        spriteRenderer.flipX = currentFacing < 0;

        attackPivot.localScale = new Vector3(
            currentFacing < 0 ? -1f : 1f,
            1f,
            1f
        );

        isTurning = false;
    }




    IEnumerator AttackRoutine(Collider2D playerCollider)
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.25f);

        PlayerHealth health = playerCollider.GetComponent<PlayerHealth>();
        if (health != null)
        {
            Vector2 hitDir = (playerCollider.transform.position - transform.position).normalized;
            health.TakeDamage(damage, hitDir);
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    public bool IsFacingSword(Vector3 swordPosition)
    {
        float dirToSword = swordPosition.x - transform.position.x;
        float facingDir = spriteRenderer.flipX ? -1f : 1f;

        // mismo lado = escudo
        return Mathf.Sign(dirToSword) == Mathf.Sign(facingDir);
    }

    public void SetDead()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
    }

    public void TryAttack(Collider2D playerCollider)
    {
        if (attackTimer > 0f || isAttacking || isDead) return;

        attackTimer = attackCooldown;
        StartCoroutine(AttackRoutine(playerCollider));
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }


    IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        spriteRenderer.color = Color.white;
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
