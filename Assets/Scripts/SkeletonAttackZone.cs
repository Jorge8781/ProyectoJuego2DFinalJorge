using UnityEngine;

public class SkeletonAttackZone : MonoBehaviour
{
    private EnemySkeletonController skeleton;

    void Start()
    {
        skeleton = GetComponentInParent<EnemySkeletonController>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        skeleton.TryAttack(other);
    }
}

