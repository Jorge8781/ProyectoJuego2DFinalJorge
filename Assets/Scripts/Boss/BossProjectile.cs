using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public static int activeProjectiles = 0;
    public float speed = 4f;
    public int damage = 1;
    public float maxLifeTime = 6f;

    BossController boss;

    void Start()
    {
        boss = FindObjectOfType<BossController>();
        boss.OnProjectileSpawned();
    }

    void KillSelf()
    {
        Destroy(gameObject);
    }




    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    void OnDestroy()
    {
        boss.OnProjectileDestroyed();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                health.TakeDamage(damage, dir);
            }
            Destroy(gameObject);
        }

        if (other.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }

    }
}


