using UnityEngine;

public class Bala : MonoBehaviour
{
    [SerializeField] private float velocidad;

    private bool clavada = false;

    public Vector2 direccion;

    private void Update()
    {
        if (!clavada)
        {
            transform.Translate(direccion * velocidad * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            clavada = true;

            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero; // detener cualquier movimiento
                rb.angularVelocity = 0f;   // detener rotación
                rb.isKinematic = true;
            }

            // Registrar esta bala como la única clavada
            FindObjectOfType<DisparoJugador>().RegistrarBalaClavada(gameObject);
        }
    }
}
