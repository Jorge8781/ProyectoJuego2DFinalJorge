using UnityEngine;

public class BossGroundHand : MonoBehaviour
{
    private Collider2D col;
    private Animator anim;

    public Transform spawnPoint;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        col.enabled = false;
    }

    // El boss llama a esto
    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

    // 🔥 Llamado desde Animation Event
    public void EnableDamage()
    {
        col.enabled = true;
    }

    // 🔥 Llamado desde Animation Event
    public void DisableDamage()
    {
        col.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            Vector2 dir = (other.transform.position - transform.position).normalized;
            health.TakeDamage(1, dir);
        }
    }
}




