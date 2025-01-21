using UnityEngine;

public class Rat : Enemy
{
    [Header("Movement Settings")]
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private float wanderDuration = 3f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float obstacleDetectionDistance = 1f; //distance to detect obstacles

    private float stateTimer; //timer for state transitions
    private float attackCooldownTimer;
    private Vector2 wanderDirection;

    private bool isWandering;
    private bool isChasing;

    private Animator animator;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        stateTimer = idleDuration;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        base.Update();

        if (currentHealth <= 0) return;

        attackCooldownTimer -= Time.deltaTime;

        if (ShouldChase())
        {
            if (!isChasing)
            {
                isChasing = true;
                animator.SetBool("IsMoving", true);
            }
            ChasePlayer();
        }
        else
        {
            if (isChasing)
                ResetToIdle();
            HandleWandering();
        }
    }

    private void HandleWandering()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            isWandering = !isWandering;
            stateTimer = isWandering ? wanderDuration : idleDuration;

            if (isWandering)
            {
                wanderDirection = new Vector2(Random.Range(0, 2) == 0 ? -1f : 1f, 0);
                Flip(wanderDirection.x);
                animator.SetBool("IsMoving", true);
            }
            else
            {
                wanderDirection = Vector2.zero;
                animator.SetBool("IsMoving", false);
            }
        }

        if (isWandering)
        {
            if (IsObstacleInPath())
                JumpOverObstacle();
            else
                transform.Translate(speed * Time.deltaTime * wanderDirection);
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;

        if (IsObstacleInPath())
            JumpOverObstacle();
        else
        {
            rb.velocity = new Vector2(direction.x * speed * 2, rb.velocity.y);
            Flip(direction.x);
        }
    }

    private bool IsObstacleInPath()
    {
        //check for obstacles in front of the enemy using a raycast
        Vector2 origin = transform.position;
        Vector2 direction = new(transform.localScale.x, 0); //forward direction based on facing
        return Physics2D.Raycast(origin, direction, obstacleDetectionDistance, groundLayer);
    }

    private void JumpOverObstacle() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    
    private void ResetToIdle()
    {
        isChasing = false;
        stateTimer = idleDuration;
        isWandering = false;
        wanderDirection = Vector2.zero;
        animator.SetBool("IsMoving", false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && attackCooldownTimer <= 0f)
        {
            Attack();
            attackCooldownTimer = attackCooldown;
        }
    }

    public override void Attack()
    {
        player.GetComponent<Player>().TakeDamage(damage);
        animator.SetTrigger("Attack");
    }

    private void Flip(float direction)
    {
        if (Mathf.Approximately(direction, 0)) return;
        transform.localScale = new Vector3(Mathf.Sign(direction) * 1.2f, transform.localScale.y, transform.localScale.z);
    }
}