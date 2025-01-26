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
    [SerializeField] private AudioClip playerChaseClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;

    private float stateTimer; //timer for state transitions
    private Vector2 wanderDirection;

    private bool isWandering;
    private bool isChasing;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        stateTimer = idleDuration;
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (currentHealth <= 0 || isKnockedBack) return;

        if (ShouldChase())
        {
            if (!isChasing)
            {
                isChasing = true;
                animator.SetBool("IsMoving", true);
                AudioManager.instance.PlaySFX(playerChaseClip);
            }
            ChasePlayer();
        }
        else
        {
            if (isChasing)
                ResetToIdle();
            HandleWandering();
        }

        //attack behavior
        if (!isAttacking && ShouldAttack())
            Attack();
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
    
    private void ResetToIdle()
    {
        isChasing = false;
        stateTimer = idleDuration;
        isWandering = false;
        wanderDirection = Vector2.zero;
        animator.SetBool("IsMoving", false);
    }

    public override void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        player.GetComponent<Player>().TakeDamage(damage);
        AudioManager.instance.PlaySFX(enemyAttack);
        AudioManager.instance.PlaySFX(attackClip);
        animator.SetTrigger("Attack");
    }

    protected override void Die()
    {
        base.Die();
        AudioManager.instance.PlaySFX(deathClip);
    }

    private void Flip(float direction)
    {
        if (Mathf.Approximately(direction, 0)) return;
        transform.localScale = new Vector3(Mathf.Sign(direction) * 1.2f, transform.localScale.y, transform.localScale.z);
    }
}