using UnityEngine;

public class PlayerEspadaGiganteController : MonoBehaviour
{
    [SerializeField] private GameObject espadaGigante;
    [SerializeField] private Transform controladorEspadaGigante;
    [SerializeField] private float duracionGolpe = 0.5f;
    [SerializeField] private Collider2D swordCollider;


    private Animator espadaAnimator;
    private float timer;
    private bool activa;

    void Awake()
    {
        espadaAnimator = espadaGigante.GetComponent<Animator>();
        espadaGigante.SetActive(false);
    }

    void Update()
    {
        if (!activa) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            DesactivarEspadaGigante();
        }
    }



    // 🔥 Llamado desde PlayerController
    public void ActivarEspadaGigante(float direccionX)
    {
        if (activa) return;

        activa = true;
        timer = duracionGolpe;

        espadaGigante.transform.position = controladorEspadaGigante.position;

        // 🔥 VOLTEO CORRECTO
        Vector3 scale = espadaGigante.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direccionX);
        espadaGigante.transform.localScale = scale;

        espadaGigante.SetActive(true);
        swordCollider.enabled = true;

        espadaAnimator.Play("EspadaGiganteAttack", 0, 0f);
    }

    private void DesactivarEspadaGigante()
    {
        activa = false;
        swordCollider.enabled = false;
        espadaGigante.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemySkeletonHealth health =
            other.GetComponent<EnemySkeletonHealth>();

        if (health == null) return;

        health.TakeDamage(999); // 💀
    }


}



