using UnityEngine;
using System.Collections;

public class EnemySkeletonHealth : MonoBehaviour
{
    public int maxHealth = 3;

    private int currentHealth;
    private bool isDead;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private Coroutine hitFlashCoroutine;

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }


    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");

        GetComponent<EnemySkeletonController>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 1.2f);
    }
}

