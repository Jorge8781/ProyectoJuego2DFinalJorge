using UnityEngine;
using System.Collections;

public class Bala : MonoBehaviour
{
    [SerializeField] private float velocidad = 10f;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool clavada = false;

    private SwordButton buttonClavado; // 🔥 CLAVE

    [SerializeField] private float alturaMaxRebote = 1f;


    private float alturaInicioRebote;
    private bool enRebote = false;


    [SerializeField] private float tiempoAntesDeDestruir = 3f;


    private bool esperandoDestruccion = false;
    private Coroutine destruirCoroutine;

    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

    }

    public void Lanzar(Vector2 dir)
    {
        rb.velocity = dir * velocidad;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (clavada) return;

        EnemySkeletonController skeleton =
     collision.collider.GetComponentInParent<EnemySkeletonController>();

        if (skeleton != null)
        {
            if (skeleton.IsFacingSword(transform.position))
            {
                RebotarContraEscudo(collision); // escudo
            }
            else
            {
                Destroy(gameObject); // aquí luego puedes hacer daño
            }
            return;
        }



        if (collision.collider.CompareTag("Wall"))
        {
            Clavarse(collision);
            return;
        }

        SwordButton button =
            collision.collider.GetComponentInParent<SwordButton>();

        if (button != null)
        {
            buttonClavado = button;
            Clavarse(collision);
            button.SetSword(this);
            return;
        }

        if (collision.collider.CompareTag("Door"))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            rb.isKinematic = false;
            rb.velocity = Vector2.up * 10f;
            Debug.Log("TEST UP");
        }
    }

    void Clavarse(Collision2D collision)
    {
        clavada = true;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        col.enabled = false;
    }

    void RebotarContraEscudo(Collision2D collision)
    {
        clavada = false;
        enRebote = true;

        alturaInicioRebote = transform.position.y;

        col.enabled = false;

        rb.isKinematic = false;
        rb.velocity = Vector2.zero;

        transform.position += Vector3.up * 0.15f;

        rb.AddForce(Vector2.up * velocidad * 3f, ForceMode2D.Impulse);

        animator.SetTrigger("Rebote"); // 🎬 AQUÍ

        StartCoroutine(ReactivarCollider());
    }




    IEnumerator ReactivarCollider()
    {
        yield return new WaitForSeconds(0.12f);
        col.enabled = true;
    }



    private void OnDestroy()
    {
        if (buttonClavado != null)
        {
            buttonClavado.RemoveSword(this); // 🔥 SOLO SU BOTÓN
        }
    }

    void FixedUpdate()
    {
        if (!enRebote) return;

        if (transform.position.y >= alturaInicioRebote + alturaMaxRebote)
        {
            rb.velocity = Vector2.zero;
            enRebote = false;

            // empezar cuenta atrás
            if (!esperandoDestruccion)
            {
                destruirCoroutine = StartCoroutine(DestruirTrasTiempo());
            }
        }
    }

    IEnumerator DestruirTrasTiempo()
    {
        esperandoDestruccion = true;

        yield return new WaitForSeconds(tiempoAntesDeDestruir);

        Destroy(gameObject);
    }

    public void CancelarAutodestruccion()
    {
        if (destruirCoroutine != null)
        {
            StopCoroutine(destruirCoroutine);
            destruirCoroutine = null;
        }

        esperandoDestruccion = false;
    }

}



