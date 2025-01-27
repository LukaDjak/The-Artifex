using UnityEngine;

public class Goblin : Enemy
{
    [Header("Goblin Attributes")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private float wanderDuration = 3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float obstacleDetectionDistance = 0.5f;

    private float stateTimer;
    private bool isWandering;
    private Vector2 wanderDirection;
    private bool isFacingRight = true;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        StartIdle();
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || isKnockedBack || isAttacking)
            return;

        if (ShouldAttack())
        {
            rb.velocity = Vector2.zero; // Stop movement during attack
            Attack();
        }
        else if (ShouldChase())
        {
            ChasePlayer();
        }
        else
        {
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
                // Pick a random direction (-1 for left, 1 for right)
                wanderDirection = new Vector2(Random.Range(0, 2) == 0 ? -1f : 1f, 0);
                Flip(wanderDirection.x);
                animator.SetBool("IsMoving", true);
            }
            else
            {
                // Stop wandering
                wanderDirection = Vector2.zero;
                animator.SetBool("IsMoving", false);
            }
        }

        if (isWandering)
        {
            if (IsObstacleInPath())
                JumpOverObstacle();
            else
                rb.velocity = new Vector2(wanderDirection.x * speed, rb.velocity.y);
        }
        else
        {
            // Stop movement when idle
            rb.velocity = new Vector2(0, rb.velocity.y);
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

    public override void Attack()
    {
        if (isAttacking)
            return;

        isAttacking = true;
        attackTimer = attackCooldown;

        rb.velocity = Vector2.zero;
        animator.SetTrigger("Attack");
        AudioManager.instance.PlaySFX(enemyAttack);

        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
            player.GetComponent<Player>().TakeDamage(damage);
    }

    private void Flip(float directionX)
    {
        bool shouldFaceRight = directionX > 0;
        if (shouldFaceRight != isFacingRight)
        {
            isFacingRight = shouldFaceRight;
            transform.localScale = new Vector3(isFacingRight ? 2 : -2, 2, 2);
        }
    }

    private void StartIdle()
    {
        isWandering = false;
        stateTimer = idleDuration;
        animator.SetBool("IsMoving", false);
    }

    protected override void Die()
    {
        if(isDead) return;
        base.Die();
        animator.SetTrigger("Death");
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        Destroy(gameObject, 5f);
    }
}