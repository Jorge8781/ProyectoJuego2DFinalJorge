using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (activated) return;

        activated = true;

        animator.SetTrigger("Activate");

        CheckpointManager.Instance.SetCheckpoint(transform.position);

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.FullHeal();
        }
    }
}




