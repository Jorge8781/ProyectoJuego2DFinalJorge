using UnityEngine;
using System.Collections;


public class EnemyGhostController : MonoBehaviour
{
    [Header("Movement")]
    public float normalSpeed = 1.5f;
    public float chaseSpeed = 5f;

    [Header("Detection")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;

    [Header("Attack")]
    public int damage = 1;
    public float attackCooldown = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private float attackTimer;
    private bool chasing;
    private SpriteRenderer spriteRenderer;

    private bool isAttacking;
    private bool isDead;
    private Animator animator;
   



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackTimer = 0f;
        animator = GetComponent<Animator>();
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
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;

        if (direction.x != 0)
            spriteRenderer.flipX = direction.x < 0;
    }




    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (attackTimer > 0f || isAttacking || isDead) return;

        attackTimer = attackCooldown;
        StartCoroutine(AttackRoutine(other));
    }


    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(
            animator.GetCurrentAnimatorStateInfo(0).length
        );
        isAttacking = false;
    }


    IEnumerator AttackRoutine(Collider2D playerCollider)
    {
        isAttacking = true;

        // 🔒 fijar dirección SOLO una vez
        FacePlayer();

        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Attack");

        // ⏱️ esperar al momento del golpe
        yield return new WaitForSeconds(0.25f);

        PlayerHealth health = playerCollider.GetComponent<PlayerHealth>();
        if (health != null)
        {
            Vector2 hitDir = (playerCollider.transform.position - transform.position).normalized;
            health.TakeDamage(damage, hitDir);
        }

        // ⏱️ esperar a que termine la animación
        yield return new WaitForSeconds(0.35f);

        isAttacking = false;
    }





    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    public void SetDead()
    {
        isDead = true;
    }

    void FacePlayer()
    {
        if (player == null) return;

        float dir = player.position.x - transform.position.x;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir);
        transform.localScale = scale;
    }
}
