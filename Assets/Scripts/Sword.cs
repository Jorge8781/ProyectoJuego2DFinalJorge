using UnityEngine;

public class SwordStick : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool stuck = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stuck) return;

        // Si tocamos una pared
        if (collision.gameObject.CompareTag("Wall"))
        {
            StickInWall();
        }
    }

    void StickInWall()
    {
        stuck = true;

        // Detener la espada
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false; // deja de recibir f�sicas

        // Opcional: congelar posici�n exactamente donde choc�
        transform.position = transform.position;
    }
}
