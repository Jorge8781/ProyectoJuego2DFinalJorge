using UnityEngine;

public class DisparoJugador : MonoBehaviour
{
    [SerializeField] private Transform ControladorDisparo;
    [SerializeField] private GameObject bala;

    private GameObject balaClavada;

    private bool tieneEspada = true;
    private Animator animator;

    public bool TieneEspada => tieneEspada; // <-- NUEVO

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void LanzarEspadaEvent()
    {
        Disparar();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Throw");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Teletransportar();
        }
    }

    private void Teletransportar()
    {
        if (balaClavada != null)
        {
            transform.position = balaClavada.transform.position;

            Destroy(balaClavada);
            balaClavada = null;

            // recuperar espada
            tieneEspada = true;
            animator.SetBool("TieneEspada", true);
        }
    }

    private void Disparar()
    {
        tieneEspada = false;
        animator.SetBool("TieneEspada", false);

        if (balaClavada != null)
        {
            Destroy(balaClavada);
            balaClavada = null;
        }

        GameObject nuevaBala = Instantiate(bala, ControladorDisparo.position, ControladorDisparo.rotation);

        balaClavada = nuevaBala;

        Bala balaScript = nuevaBala.GetComponent<Bala>();

        if (transform.localScale.x < 0)
        {
            balaScript.direccion = Vector2.left;
            nuevaBala.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            balaScript.direccion = Vector2.right;
            nuevaBala.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void RegistrarBalaClavada(GameObject bala)
    {
        balaClavada = bala;
    }
}

