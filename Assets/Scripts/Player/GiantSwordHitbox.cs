using UnityEngine;

public class GiantSwordHitbox : MonoBehaviour
{
    public int damage = 3;
    private bool hasHit;

    void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        EnemySkeletonController skeleton =
            other.GetComponentInParent<EnemySkeletonController>();

        if (skeleton != null)
        {
            Transform player = FindObjectOfType<PlayerController>().transform;

            if (skeleton.IsFacingSword(player.position))
            {
                hasHit = true;
                return;
            }

            skeleton.TakeSwordDamageFromPosition(damage, player.position);
            hasHit = true;
            return;
        }


        EnemyGhostHealth ghost =
            other.GetComponentInParent<EnemyGhostHealth>();

        if (ghost != null)
        {
            ghost.TakeDamage(damage);
            hasHit = true;
        }

        BossHealth boss = other.GetComponentInParent<BossHealth>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            hasHit = true;
        }

    }



}





