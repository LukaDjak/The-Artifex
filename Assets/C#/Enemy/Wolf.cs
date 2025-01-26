using System.Collections;
using UnityEngine;

public class Wolf : Enemy
{
    [Header("Wolf Attributes")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float runDuration = 5f;
    [SerializeField] private float idleTime = 15f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float obstacleDetectionDistance = 1f;
    [SerializeField] private AudioClip[] deathClips;

    private Animator animator;
    private bool isRunning;
    private bool isChasing;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(IdleBehavior());
    }

    protected override void Update()
    {
        base.Update();
        if (currentHealth <= 0 || isKnockedBack || isDead) return;

        //chase behavior
        if (ShouldChase())
        {
            isChasing = true;
            ChasePlayer();
        }
        else
            isChasing = false;

        //attack behavior
        if (!isAttacking && ShouldAttack())
            Attack();

        //update animation states
        animator.SetBool("Walk", isRunning || isChasing);
    }

    private void FixedUpdate()
    {
        if(isKnockedBack || isDead) return;
        if (!isChasing && isRunning)
        {
            if (IsObstacleInPath() && !isKnockedBack)
                JumpOverObstacle();
            else
                rb.velocity = new Vector2(speed * (transform.localScale.x > 0 ? 1 : -1), rb.velocity.y);
        }
        else if (!isChasing && !isRunning)
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private IEnumerator IdleBehavior()
    {
        while (true)
        {
            //idle state
            isRunning = false;
            yield return new WaitForSeconds(idleTime);

            //running state
            isRunning = true;
            yield return new WaitForSeconds(runDuration);
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;

        if (IsObstacleInPath())
            JumpOverObstacle();
        else
        {
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
            Flip(direction.x);
        }
    }

    private void Flip(float direction)
    {
        if ((direction > 0 && transform.localScale.x < 0) || (direction < 0 && transform.localScale.x > 0))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private bool IsObstacleInPath()
    {
        //check for obstacles in front of the wolf using a raycast
        Vector2 origin = transform.position + new Vector3(0, -.2f, 0);
        Vector2 direction = new(transform.localScale.x, 0);
        return Physics2D.Raycast(origin, direction, obstacleDetectionDistance, groundLayer);
    }

    private void JumpOverObstacle() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    
    public override void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        animator.SetTrigger("Attack");
        AudioManager.instance.PlaySFX(enemyAttack);

        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
            player.GetComponent<Player>().TakeDamage(damage);
    }

    protected override void Die()
    {
        if(isDead) return;
        base.Die();
        animator.SetTrigger("Death");
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        AudioManager.instance.PlaySFX(deathClips[Random.Range(0, deathClips.Length)]);
        Destroy(gameObject, 5f);
    }
}