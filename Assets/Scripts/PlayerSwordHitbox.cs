using UnityEngine;

public class PlayerSwordHitbox : MonoBehaviour
{
    public int damage = 1;
    public int giantSwordDamage = 3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemySkeletonController skeleton =
            other.GetComponent<EnemySkeletonController>();

        EnemySkeletonHealth health =
            other.GetComponent<EnemySkeletonHealth>();

        if (skeleton == null || health == null) return;

        // ?? Si viene de frente ? NO daño
        if (skeleton.IsFacingSword(transform.position))
            return;

        health.TakeDamage(damage);
    }
}

