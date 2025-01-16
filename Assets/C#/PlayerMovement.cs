using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float          speed = 4.0f;
    [SerializeField] private float          jumpForce = 7.5f;
    [SerializeField] private LayerMask      groundLayer;

    [Header("Jump Mechanics")]
    [SerializeField] private float          coyoteTime = 0.2f;
    [SerializeField] private float          jumpBufferTime = 0.2f;

    private Rigidbody2D                     rb;
    private Animator                        animator;

    private float                           xInput;
    private float                           coyoteCounter;
    private float                           jumpBufferCounter;
    private bool                            isGrounded;
    private bool                            facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        isGrounded = Physics2D.CircleCast(transform.position, .3f, Vector2.down, 0.1f, groundLayer);

        //coyote time
        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        //jump buffer
        jumpBufferCounter = Input.GetButtonDown("Jump") ? jumpBufferTime : jumpBufferCounter - Time.deltaTime;

        //player flip
        if ((xInput > 0 && !facingRight) || (xInput < 0 && facingRight))
            Flip();

        //jump
        if (jumpBufferCounter > 0 && coyoteCounter > 0)
            Jump();

        //update animator parameters
        animator.SetFloat("AirSpeed", rb.velocity.y);
        animator.SetBool("Grounded", isGrounded);
        animator.SetInteger("AnimState", Mathf.Abs(xInput) > Mathf.Epsilon ? 2 : 0);

        //TEMP - ANIM TESTING
        if (Input.GetKeyDown(KeyCode.R)) animator.SetTrigger("Attack");
        if (Input.GetKeyDown(KeyCode.T)) animator.SetTrigger("Recover");
    }

    private void FixedUpdate() => rb.velocity = new Vector2(xInput * speed, rb.velocity.y);

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        coyoteCounter = 0;
        jumpBufferCounter = 0;
        animator.SetTrigger("Jump");
        if (rb.velocity.y == 12 && !isGrounded) //some kind of effect for double jump
            Debug.Log("secondJump");
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}