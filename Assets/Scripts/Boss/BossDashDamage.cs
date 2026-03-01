using UnityEngine;

public class BossDashDamage : MonoBehaviour
{
    public int damage = 1;

    private BossController boss;

    private bool hasHitPlayer;

    void Update()
    {
        if (!boss.IsDashing)
            hasHitPlayer = false;
    }


    void Awake()
    {
        boss = GetComponentInParent<BossController>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!boss.IsDashing) return;
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // 🔥 INVULNERABILIDAD DURANTE DASH
        if (player.IsDashing) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        Vector2 dir = (other.transform.position - transform.position).normalized;
        health.TakeDamage(damage, dir);
    }


}
