using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damageWithSword = 2;
    public int damageWithoutSword = 1;

    private DisparoJugador disparo;

    void Awake()
    {
        disparo = FindObjectOfType<DisparoJugador>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!gameObject.activeInHierarchy) return;

        int damage = disparo.TieneEspada ? damageWithSword : damageWithoutSword;

        BossHealth boss = other.GetComponentInParent<BossHealth>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            return;
        }

        EnemyGhostHealth ghost = other.GetComponentInParent<EnemyGhostHealth>();
        if (ghost != null)
        {
            ghost.TakeDamage(damage);
            return;
        }

        EnemySkeletonController skeleton = other.GetComponentInParent<EnemySkeletonController>();
        if (skeleton != null)
        {
            skeleton.TakeSwordDamageFromPosition(damage, transform.position);
        }
    }
}








