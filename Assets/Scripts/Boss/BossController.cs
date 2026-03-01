using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    // ───────────── MOVIMIENTO ─────────────
    [Header("Movement")]
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform centerPoint;

    public float dashSpeed = 20f;
    public float waitAfterDash = 0.5f;

    private bool isDashing;


    public bool IsVulnerable { get; private set; }


    // ───────────── ACTIVACIÓN ─────────────
    [Header("Activation")]
    public float detectionRadius = 6f;
    public LayerMask playerLayer;

    private Transform player;
    private bool active;

    // ───────────── FIRE PROJECTILES ─────────────
    [Header("Fire Projectiles")]
    public GameObject projectilePrefab;
    public float shootDuration = 3f;
    public float shootInterval = 0.4f;

    [Header("Fire Area")]
    public Transform fireLeftLimit;
    public Transform fireRightLimit;
    public float projectileSpawnHeight = 6f;

    private int projectilesAlive = 0;

    // ───────────── GROUND HANDS ─────────────
    [Header("Ground Hands")]
    public GameObject groundHandPrefab;
    public float handsDuration = 3f;
    public float handInterval = 0.5f;

    [Header("Hands Area")]
    public Transform handsLeftLimit;
    public Transform handsRightLimit;

    [Header("Hands Spread")]
    public float minDistanceBetweenHands = 1.5f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float raycastHeight = 10f;

    private List<BossGroundHand> activeHands = new List<BossGroundHand>();
    private List<float> usedHandPositions = new List<float>();

    public bool IsDashing => isDashing;

    private SpriteRenderer sr;
    private Color originalColor;




    // ───────────── PATRONES ─────────────
    public enum BossPattern
    {
        Projectiles,
        GroundHands
    }

    public BossPattern currentPattern;

    private Animator anim;

    // ───────────── UNITY ─────────────
    void Start()
    {
        currentPattern = BossPattern.Projectiles;
        anim = GetComponentInChildren<Animator>();


        sr = GetComponentInChildren<SpriteRenderer>();
        originalColor = sr.color;
    }


    void Update()
    {
        if (active) return;

        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            detectionRadius,
            playerLayer
        );

        if (hit)
        {
            player = hit.transform;
            active = true;

            // 🔥 MOSTRAR BARRA
            BossHealthBar.Instance.Show();

            // 🔥 INICIALIZAR VIDA
            BossHealth health = GetComponent<BossHealth>();
            BossHealthBar.Instance.UpdateBar(
                health.GetCurrentHealth(),
                health.GetMaxHealth()
            );

            StartCoroutine(BossLoop());
        }


    }

    // ───────────── LOOP PRINCIPAL ─────────────
    IEnumerator BossLoop()
    {
        while (true)
        {
            yield return Dash(leftPoint.position);
            yield return new WaitForSeconds(waitAfterDash);

            yield return Dash(rightPoint.position);
            yield return new WaitForSeconds(waitAfterDash);

            yield return Dash(centerPoint.position);
            yield return VulnerableWindow();   // ⬅️ aquí está la clave

            if (currentPattern == BossPattern.Projectiles)
            {
                yield return ShootPhase();
                currentPattern = BossPattern.GroundHands;
            }
            else
            {
                yield return GroundHandsPhase();
                currentPattern = BossPattern.Projectiles;
            }

            yield return new WaitForSeconds(0.6f);
        }
    }

    // ───────────── DASH ─────────────
    IEnumerator Dash(Vector2 to)
    {
        isDashing = true;
        anim.SetBool("IsDashing", true);

        Vector2 dir = (to - (Vector2)transform.position).normalized;

        // 🔥 FLIP CORREGIDO
        if (dir.x != 0)
            sr.flipX = dir.x > 0;

        while (Vector2.Distance(transform.position, to) > 0.1f)
        {
            transform.position += (Vector3)(dir * dashSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = to;

        isDashing = false;
        anim.SetBool("IsDashing", false);
    }



    IEnumerator VulnerableWindow()
    {
        IsVulnerable = true;

        // animación de cansado / stunned
        anim.SetTrigger("Vulnerable");

        yield return new WaitForSeconds(3f); // tiempo vulnerable

        IsVulnerable = false;
    }


    public void Die()
    {
        StopAllCoroutines();

        IsVulnerable = false;

        anim.SetTrigger("Death");

        // Desactivar collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Desactivar física
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = false;

        // Ocultar barra
        BossHealthBar.Instance.gameObject.SetActive(false);

        StartCoroutine(WaitForDeathAnimation());
    }


    IEnumerator WaitForDeathAnimation()
    {
        yield return null; // esperar 1 frame para que entre en Death

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        while (!state.IsName("Death"))
        {
            yield return null;
            state = anim.GetCurrentAnimatorStateInfo(0);
        }

        yield return new WaitForSeconds(state.length);

        Destroy(gameObject);
    }




    // ───────────── FIRE PHASE ─────────────
    IEnumerator ShootPhase()
    {
        // 🔥 iniciar secuencia de grito
        anim.SetTrigger("StartShout");
        anim.SetBool("IsShouting", true);

        // ⏳ esperar a que terminen las animaciones de girar cabeza
        yield return new WaitForSeconds(0.6f);
        // ajusta a la duración REAL de HeadTurn

        float timer = 0f;

        while (timer < shootDuration)
        {
            SpawnProjectile();
            timer += shootInterval;
            yield return new WaitForSeconds(shootInterval);
        }

        // 🛑 parar grito
        anim.SetBool("IsShouting", false);

        // esperar a que caigan los últimos fuegos
        while (projectilesAlive > 0)
            yield return null;
    }



    void SpawnProjectile()
    {
        float x = Random.Range(
            fireLeftLimit.position.x,
            fireRightLimit.position.x
        );

        Vector2 spawnPos = new Vector2(
            x,
            fireLeftLimit.position.y + projectileSpawnHeight
        );

        Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
    }

    public void OnProjectileSpawned() => projectilesAlive++;
    public void OnProjectileDestroyed() => projectilesAlive--;

    // ───────────── HANDS PHASE ─────────────
    IEnumerator GroundHandsPhase()
    {
        activeHands.Clear();
        usedHandPositions.Clear();

        // 🔥 Lanzar animación de invocación
        anim.SetTrigger("SummonHands");

        // ⏳ Esperar a que termine la animación
        yield return new WaitForSeconds(0.8f);
        // Ajusta este tiempo a la duración real de la animación

        float timer = 0f;

        while (timer < handsDuration)
        {
            SpawnHand();
            timer += handInterval;
            yield return new WaitForSeconds(handInterval);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var hand in activeHands)
            hand.Attack();

        yield return new WaitForSeconds(0.6f);

        foreach (var hand in activeHands)
            Destroy(hand.gameObject);
    }


    void SpawnHand()
    {
        float x;
        int attempts = 0;

        do
        {
            x = Random.Range(
                handsLeftLimit.position.x,
                handsRightLimit.position.x
            );
            attempts++;
        }
        while (usedHandPositions.Exists(p => Mathf.Abs(p - x) < minDistanceBetweenHands) && attempts < 10);

        if (attempts >= 10) return;

        Vector2 rayOrigin = new Vector2(x, centerPoint.position.y + raycastHeight);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 20f, groundLayer);

        if (!hit) return;

        usedHandPositions.Add(x);

        GameObject go = Instantiate(groundHandPrefab, Vector3.zero, Quaternion.identity);
        BossGroundHand hand = go.GetComponent<BossGroundHand>();

        // 🔥 mover la mano para que el SpawnPoint coincida con el suelo
        Vector3 offset = go.transform.position - hand.spawnPoint.position;
        go.transform.position = hit.point + (Vector2)offset;

        activeHands.Add(hand);
    }



    // ───────────── DEBUG ─────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}

