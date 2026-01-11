using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 6;
    public int currentHealth;

 

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerController playerController;
    private bool isDead;


    private HeartsUI heartsUI;
    private float originalGravity;

    private SpriteRenderer spriteRenderer;
    private Coroutine hitFlashCoroutine;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        spriteRenderer = GetComponent<SpriteRenderer>();

    }


    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        heartsUI = FindObjectOfType<HeartsUI>();
        heartsUI.UpdateHearts(currentHealth);
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHealth -= damage;
        heartsUI.UpdateHearts(currentHealth);

        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        spriteRenderer.color = Color.white;
    }



    void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("Death");
        playerController.OnDeath();

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1.2f);

        Vector3 spawnPos = CheckpointManager.Instance.GetCheckpointPosition();
        Respawn(spawnPos);
    }


    void Respawn(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;

        currentHealth = maxHealth;
        heartsUI.UpdateHearts(currentHealth);

        isDead = false;

        animator.Rebind();
        animator.Update(0f);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 1f;
        rb.linearVelocity = Vector2.zero;

        playerController.Revive();
    }

    public void Revive()
    {
        isDead = false;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
    }

    public void InstantKill()
    {
        if (isDead) return;

        currentHealth = 0;
        heartsUI.UpdateHearts(currentHealth);
        Die();
    }
    public void FullHeal()
    {
        currentHealth = maxHealth;
        heartsUI.UpdateHearts(currentHealth);
    }


}



