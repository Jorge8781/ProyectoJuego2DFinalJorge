using UnityEngine;

public class GiantSwordHitbox : MonoBehaviour
{
    public int damage = 2;
    private bool hasHit;

    void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (!other.CompareTag("Enemy")) return;

        EnemyGhostHealth enemy = other.GetComponentInParent<EnemyGhostHealth>();
        if (enemy == null) return;

        hasHit = true;

        // 🔥 MISMO PRINCIPIO
        Vector2 dir = (other.transform.position - transform.root.position).normalized;
        enemy.TakeDamage(damage);

    }
}



