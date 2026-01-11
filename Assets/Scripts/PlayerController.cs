using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 4f;

    private float move;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Wall Slide")]
    public Transform wallCheck;
    public float wallCheckRadius = 0.1f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = -1.5f;

    private bool isTouchingWall;
    private bool isWallSliding;

    [Header("Wall Jump")]
    public float wallJumpForce = 6f;
    public float wallJumpDuration = 0.15f;

    private bool isWallJumping;
    private float wallJumpTimer;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.15f;

    private bool isDashing;
    private float dashTimer;
    private float originalGravity;

    [Header("Attack Combo")]
    public float attack1Duration = 0.4f;
    public float attack2Duration = 0.4f;
    public float attack3Duration = 0.6f;
    public float comboInputWindow = 0.25f;

    private int attackIndex;
    private float attackTimer;
    private bool isAttacking;
    private bool queuedNextAttack;
    private bool inputBuffered;

    private DisparoJugador disparo;
    private PlayerEspadaGiganteController espadaGiganteController;
    private bool comboWindowOpen;

    [SerializeField] private Collider2D swordCollider;
    private bool espadaGiganteLanzada;

    private bool isDead;





    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        disparo = GetComponent<DisparoJugador>();
        espadaGiganteController = GetComponent<PlayerEspadaGiganteController>();

        originalGravity = rb.gravityScale;

        swordCollider.enabled = false;

    }

    void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleJump();
        HandleWallSlide();
        HandleWallJump();
        HandleDash();
        HandleAttack();
        UpdateAnimator();
    }


    // ───────── MOVIMIENTO ─────────
    void HandleMovement()
    {
        if (isAttacking || isDashing) return;

        move = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // ───────── WALL SLIDE ─────────
    void HandleWallSlide()
    {
        if (isTouchingWall && !isGrounded && !isWallJumping)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }
    }

    // ───────── WALL JUMP ─────────
    void HandleWallJump()
    {
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;
            wallJumpTimer = wallJumpDuration;

            Vector2 dir = new Vector2(-transform.localScale.x, 1).normalized;
            rb.linearVelocity = dir * wallJumpForce;

            transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
        }

        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
                isWallJumping = false;
        }
    }

    // ───────── DASH ─────────
    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && !isAttacking)
        {
            isDashing = true;
            dashTimer = dashDuration;
            rb.gravityScale = 0;
            animator.SetTrigger("Dash");
        }

        if (isDashing)
        {
            rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
                rb.gravityScale = originalGravity;
            }
        }
    }

    // ───────── ATAQUE / COMBO ─────────
    void HandleAttack()
    {
        if (!disparo.TieneEspada || isDashing) return;

        // INPUT
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                StartAttack(1);
            }
            else if (comboWindowOpen)
            {
                queuedNextAttack = true;
            }
        }



        if (!isAttacking) return;

        attackTimer -= Time.deltaTime;

        if (!comboWindowOpen && attackTimer <= comboInputWindow)
        {
            comboWindowOpen = true;
        }

        if (attackTimer <= 0f)
        {
            if (queuedNextAttack && attackIndex < 3)
            {
                StartAttack(attackIndex + 1);
            }
            else
            {
                EndAttack();
            }
        }

        if (isAttacking && attackIndex == 3 && !espadaGiganteLanzada)
        {
            espadaGiganteLanzada = true;
            StartCoroutine(ActivarEspadaGiganteDelay());
        }

    }



    void StartAttack(int index)
    {
        isAttacking = true;
        attackIndex = index;
        queuedNextAttack = false;
        comboWindowOpen = false;

        rb.linearVelocity = Vector2.zero;

        switch (attackIndex)
        {
            case 1: attackTimer = attack1Duration; break;
            case 2: attackTimer = attack2Duration; break;
            case 3: attackTimer = attack3Duration; break;
        }

        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetTrigger("Attack");

        StartCoroutine(EnableSwordHitbox()); // 🔥 CLAVE

        if (attackIndex == 3)
            StartCoroutine(ActivarEspadaGiganteDelay());
    }



    IEnumerator EnableSwordHitbox()
    {
        Debug.Log("HITBOX ON");
        swordCollider.enabled = true;
        yield return new WaitForSeconds(0.5f);
        swordCollider.enabled = false;
        Debug.Log("HITBOX OFF");
    }




    IEnumerator ActivarEspadaGiganteDelay()
    {
        yield return new WaitForSeconds(0.15f);

        float direccion = transform.localScale.x;
        espadaGiganteController.ActivarEspadaGigante(direccion);
    }



    void EndAttack()
    {
        isAttacking = false;
        attackIndex = 0;
        queuedNextAttack = false;

        animator.SetInteger("AttackIndex", 0);
    }





    // ───────── ANIMATOR ─────────
    void UpdateAnimator()
    {
        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsWallSliding", isWallSliding);
        animator.SetBool("TieneEspada", disparo.TieneEspada);
    }

    // ───────── PHYSICS ─────────
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }

    public void OnDeath()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        isAttacking = false;
        isDashing = false;
    }
    public void Revive()
    {
        isDead = false;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
    }


}
