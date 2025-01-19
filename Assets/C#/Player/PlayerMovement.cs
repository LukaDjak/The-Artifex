using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 4.0f;
    public float jumpForce = 7.5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float dashSpeed = 150f;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private float xInput, coyoteCounter, jumpBufferCounter, currentDashTime;
    private bool isGrounded, isDashing, canDash, isSprinting, facingRight = true;
    private Player player;

    [HideInInspector] public bool dashActive = false;

    [Header("Jump Mechanics")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!LevelManager.instance.IsGameOver())
        {
            xInput = Input.GetAxis("Horizontal");
            isGrounded = Physics2D.CircleCast(transform.position, .3f, Vector2.down, 0.1f, groundLayer);

            //coyote Time
            coyoteCounter = isGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;

            //jump Buffer
            if (Input.GetButtonDown("Jump"))
                jumpBufferCounter = jumpBufferTime;

            if (jumpBufferCounter > 0 && coyoteCounter > 0)
            {
                Jump();
                jumpBufferCounter = 0;
            }

            //sprint
            if (Input.GetKey(KeyCode.LeftShift) && xInput != 0 && player.stamina > 0)
            {
                isSprinting = true;
                player.ReduceStamina(50f); //stamina drain rate
            }
            else isSprinting = false;

            //dash
            if (Input.GetKeyDown(KeyCode.LeftControl) && !isGrounded && !isDashing && dashActive && canDash)
                StartDash();

            if (isDashing)
                Dash();

            //player flip
            if ((xInput > 0 && !facingRight) || (xInput < 0 && facingRight)) Flip();

            //animator parameters
            animator.SetFloat("AirSpeed", rb.velocity.y);
            animator.SetBool("Grounded", isGrounded);
            animator.SetInteger("AnimState", Mathf.Abs(xInput) > Mathf.Epsilon ? 2 : 0);
        }
        else
        {
            rb.velocity = Vector3.zero;
            animator.SetTrigger("Death");
        }    
    }

    private void FixedUpdate()
    {
        if (!isDashing && !LevelManager.instance.IsGameOver())
        {
            float moveSpeed = speed * (isSprinting ? sprintMultiplier : 1f);
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        coyoteCounter = 0;
        animator.SetTrigger("Jump");
        canDash = true;
    }

    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        currentDashTime = dashTime;
    }

    private void Dash()
    {
        rb.velocity = new Vector2((facingRight ? 1 : -1) * dashSpeed, rb.velocity.y);
        currentDashTime -= Time.deltaTime;
        if (currentDashTime <= 0) StopDash();
    }

    private void StopDash()
    {
        isDashing = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    //deal some damage to enemies when dashing into them
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.gameObject.TryGetComponent<Enemy>(out Enemy e))
            e.TakeDamage(10);
    }
}