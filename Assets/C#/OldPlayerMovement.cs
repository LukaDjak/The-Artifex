using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OldPlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float xInput;
    private float coyoteCounter, jumpBufferCounter;

    private bool isGrounded;
    private bool facingRight = true;
    private bool jumping;

    void Start() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        //movement input
        xInput = Input.GetAxis("Horizontal");

        //ground check
        isGrounded = Physics2D.OverlapCircle(transform.position - new Vector3(0, .5f), .4f, groundLayer);

        //coyote time & jump buffer
        if (isGrounded)
        {
            coyoteCounter = 0.2f;
            jumping = false;
        }
        else
            coyoteCounter -= Time.deltaTime;

        jumpBufferCounter = Input.GetButtonDown("Jump") ? 0.2f : jumpBufferCounter - Time.deltaTime;

        //player flip
        if ((xInput < 0 && facingRight) || (xInput > 0 && !facingRight))
            Flip();

        //jump
        if (jumpBufferCounter > 0 && coyoteCounter > 0 && !jumping)
            Jump();
    }


    void FixedUpdate() => rb.velocity = new Vector2(xInput * speed, rb.velocity.y);

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        coyoteCounter = 0;
        jumpBufferCounter = 0;
        jumping = true;
        if (rb.velocity.y == 12 && !isGrounded) //some kind of effect for double jump
            Debug.Log("secondJump");                
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}