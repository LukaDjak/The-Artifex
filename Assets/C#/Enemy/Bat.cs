using UnityEngine;

public class Bat : Enemy
{
    [Header("Bat Attributes")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float chaseSpeedMultiplier = 2f;
    [SerializeField] private float minWanderTime = 2f;
    [SerializeField] private float maxWanderTime = 5f;

    private float wanderTimer;
    private bool isFacingRight = true;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        SetRandomWanderTime();
    }

    protected override void Update()
    {
        base.Update();
        if (isDead || isKnockedBack || isAttacking) return;

        if (ShouldAttack())
        {
            rb.velocity = Vector2.zero;
            Attack();
        }
        else if (ShouldChase())
            ChasePlayer();
        else
            Wander();
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            Flip();
            SetRandomWanderTime();
        }

        //move horizontally during wandering
        rb.velocity = new Vector2((isFacingRight ? 1 : -1) * speed, 0);
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float directionX = direction.x;
        float directionY = direction.y;

        //flip based on horizontal direction
        if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
            Flip();

        //set velocity towards player
        rb.velocity = new Vector2(directionX * speed * chaseSpeedMultiplier, directionY * speed * chaseSpeedMultiplier);

        //stop when close to attack range
        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange - 0.5f)
            rb.velocity = Vector2.zero;
    }

    public override void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        attackTimer = attackCooldown;
        animator.SetTrigger("Attack");
        AudioManager.instance.PlaySFX(enemyAttack);

        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
            player.GetComponent<Player>().TakeDamage(damage);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(isFacingRight ? 4.5f : -4.5f, 4.5f, 4.5f);
    }

    private void SetRandomWanderTime() => wanderTimer = Random.Range(minWanderTime, maxWanderTime);
    
    protected override void ResetKnockback()
    {
        base.ResetKnockback();
        rb.velocity = Vector2.zero; // Stop all movement after knockback
    }

    protected override void Die()
    {
        if(isDead) return;
        base.Die();
        animator.SetTrigger("Death");
        rb.gravityScale = 1f;
        Destroy(gameObject, 5f);
    }
}