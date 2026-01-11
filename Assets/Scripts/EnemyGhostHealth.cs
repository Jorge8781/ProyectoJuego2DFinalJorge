using UnityEngine;
using System.Collections;

public class EnemyGhostHealth : MonoBehaviour
{
    public int maxHealth = 4;

    private int currentHealth;
    private Animator animator;
    private bool isDead;

    private SpriteRenderer spriteRenderer;
    private Coroutine hitFlashCoroutine;


    void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

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

        // 🔥 animación de muerte
        animator.SetTrigger("Death");

        // 🔥 desactivar lógica, NO destruir aún
        GetComponent<EnemyGhostController>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(WaitForDeathAnimation());
    }

    IEnumerator WaitForDeathAnimation()
    {
        // Esperar a entrar en Death
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Death")
        );

        // Esperar a que termine
        yield return new WaitForSeconds(
            animator.GetCurrentAnimatorStateInfo(0).length
        );

        Destroy(gameObject);
    }
}





