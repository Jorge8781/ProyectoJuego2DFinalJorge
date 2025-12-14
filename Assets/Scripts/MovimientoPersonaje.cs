using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rb2D;

    private float move;

    public float jumpForce = 4;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    private Animator animator;
    private DisparoJugador disparo;   

    [Header("Wall Slide")]
    public Transform wallCheck;
    public float wallCheckRadius = 0.1f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = -1.5f;

    private bool isTouchingWall;
    private bool isWallSliding;

    [Header("Wall Jump")]
    public float wallJumpForce = 6f;
    public Vector2 wallJumpDirection = new Vector2(1, 1);
    public float wallJumpDuration = 0.15f;

    private bool isWallJumping;
    private float wallJumpTimer;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.15f;

    private bool isDashing;
    private float dashTimer;
    private float originalGravity;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        disparo = GetComponent<DisparoJugador>();
        originalGravity = rb2D.gravityScale;
    }

    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");

        if (!isWallJumping)
            rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);

        // DASH INPUT 
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            dashTimer = dashDuration;

            rb2D.gravityScale = 0;
            animator.SetTrigger("Dash");
        }

        // DASH MOVEMENT
        if (isDashing)
        {
            float dashDir = transform.localScale.x;

            rb2D.linearVelocity = new Vector2(dashDir * dashSpeed, 0);

            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
                rb2D.gravityScale = originalGravity;
            }

            return; // ⛔ Bloquea el resto del Update
        }


        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("TieneEspada", disparo.TieneEspada);

        // WALL SLIDE (Activación inmediata al tocar pared)
        if (isTouchingWall && !isGrounded)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        // Aplicar velocidad de deslizamiento solo si se está deslizando
        if (isWallSliding)
        {
            animator.SetBool("IsWallSliding", true);

            // Limitar la caída a wallSlideSpeed
            if (rb2D.linearVelocity.y < wallSlideSpeed)
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, wallSlideSpeed);
        }
        else
        {
            animator.SetBool("IsWallSliding", false);
        }



        // WALL JUMP
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;
            wallJumpTimer = wallJumpDuration;

            animator.SetBool("IsWallJumping", true);

            Vector2 jumpDir = new Vector2(-transform.localScale.x, 1).normalized;

            rb2D.linearVelocity = new Vector2(jumpDir.x * wallJumpForce, jumpDir.y * wallJumpForce);

            transform.localScale = new Vector3(Mathf.Sign(jumpDir.x), 1, 1);
        }


        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
                animator.SetBool("IsWallJumping", false);
            }
        }


    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }
}


