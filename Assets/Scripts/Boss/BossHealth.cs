using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;

    private BossController boss;
    private SpriteRenderer sr;

    private bool canTakeHit = true;

    void Awake()
    {
        boss = GetComponent<BossController>();
        sr = GetComponentInChildren<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;


    public void TakeDamage(int damage)
    {
        if (!boss.IsVulnerable) return;
        if (!canTakeHit) return;

        currentHealth -= damage;
        BossHealthBar.Instance.UpdateBar(currentHealth, maxHealth);

        StartCoroutine(HitCooldown());
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            boss.Die();
    }

    IEnumerator HitCooldown()
    {
        canTakeHit = false;
        yield return new WaitForSeconds(0.25f); // tiempo entre golpes
        canTakeHit = true;
    }

    IEnumerator HitFlash()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }


}


