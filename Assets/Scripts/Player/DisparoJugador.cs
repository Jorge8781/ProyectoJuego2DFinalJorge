using UnityEngine;

public class DisparoJugador : MonoBehaviour
{
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private GameObject bala;
    [SerializeField] private GameObject teleportParticles;


    private GameObject balaClavada;

    private bool tieneEspada = true;
    private Animator animator;

    public bool TieneEspada => tieneEspada;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // CLICK DERECHO para lanzar espada
        if (Input.GetButtonDown("Fire2"))
        {
            LanzarEspada();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Teletransportar();
        }
    }


    private void LanzarEspada()
    {

        // Si hay una espada previa, destruirla
        if (balaClavada != null)
        {
            Destroy(balaClavada);
            balaClavada = null;
        }

        // Ahora lanzamos una nueva
        tieneEspada = false;
        animator.SetBool("TieneEspada", false);
        animator.SetTrigger("Throw");

        GameObject nuevaBala = Instantiate(
            bala,
            controladorDisparo.position,
            Quaternion.identity
        );

        // 🔥 REGISTRAR DESDE EL PRINCIPIO
        balaClavada = nuevaBala;

        Bala balaScript = nuevaBala.GetComponent<Bala>();

        if (transform.localScale.x < 0)
        {
            balaScript.Lanzar(Vector2.left);
            nuevaBala.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            balaScript.Lanzar(Vector2.right);
            nuevaBala.transform.rotation = Quaternion.identity;
        }


    }


    private void Teletransportar()
    {
        if (balaClavada == null) return;

        // 🔥 cancelar autodestrucción si existe
        Bala balaScript = balaClavada.GetComponent<Bala>();
        if (balaScript != null)
        {
            balaScript.CancelarAutodestruccion();
        }

        // Partículas en origen (jugador)
        SpawnParticles(transform.position);

        // Teleport
        transform.position = balaClavada.transform.position;

        // Partículas en destino (espada)
        SpawnParticles(balaClavada.transform.position);

        Destroy(balaClavada);
        balaClavada = null;

        tieneEspada = true;
        animator.SetBool("TieneEspada", true);
    }

    void SpawnParticles(Vector3 position)
    {
        if (teleportParticles == null) return;

        GameObject p = Instantiate(
            teleportParticles,
            position,
            Quaternion.identity
        );

        Destroy(p, 1f);
    }



    public void RegistrarBalaClavada(GameObject bala)
    {
        balaClavada = bala;
    }
}

