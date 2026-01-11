using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 🔹 FANTASMA
        EnemyGhostHealth ghost = other.GetComponent<EnemyGhostHealth>();
        if (ghost != null)
        {
            ghost.TakeDamage(damage);
            return;
        }

        // 🔹 ESQUELETO
        EnemySkeletonController skeleton =
            other.GetComponentInParent<EnemySkeletonController>();

        if (skeleton != null)
        {
            skeleton.TakeDamage(damage);
            return;
        }
    }
}





