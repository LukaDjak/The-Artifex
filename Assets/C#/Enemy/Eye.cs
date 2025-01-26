using UnityEngine;

public class Eye : Enemy
{
    [Header("Flying Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float minFlightTime = 2f;
    [SerializeField] private float maxFlightTime = 5f;

    [Header("Attack Settings")]
    [SerializeField] private int secondaryAttackChance = 20;

    [Space(15f)]
    [SerializeField] private AudioClip primaryAttackClip;
    [SerializeField] private AudioClip secondaryAttackClip;
    [SerializeField] private AudioClip deathClip;

    private float patrolDirectionTimer = 0f;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        //skip update if dead or knockedback
        if (currentHealth <= 0 || isKnockedBack || isDead) return;

        //attack logic
        if (attackTimer <= 0f && !isAttacking)
        {
            if (ShouldAttack())
                Attack();
        }

        //chase and patroll
        if (ShouldChase())
            ChasePlayer();
        else
            Patrol();
    }

    //move along X-axis with some variation on Y-axis for the flight time
    private void Patrol()
    {
        // If the timer runs out, choose a new random flight direction and duration
        if (patrolDirectionTimer <= 0f)
        {
            // Randomize direction (-1 for left, 1 for right)
            float randomDirection = Random.Range(0, 2) == 0 ? -1f : 1f;

            // Randomize flight duration between min and max flight time
            patrolDirectionTimer = Random.Range(minFlightTime, maxFlightTime);

            // Set velocity based on direction
            rb.velocity = new Vector2(randomDirection * patrolSpeed, Mathf.Sin(Time.time * 2f) * 0.5f); // Smooth Y-axis oscillation

            // Flip sprite to match direction
            Flip(randomDirection);
        }

        // Decrease the timer
        patrolDirectionTimer -= Time.deltaTime;

        // Optional: Add Y-axis oscillation to simulate flying
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Sin(Time.time * 2f) * 0.5f);
    }

    private void ChasePlayer()
    {
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= chaseRange)
        {
            float directionX = Mathf.Sign(player.transform.position.x - transform.position.x);
            float directionY = Mathf.Sign(player.transform.position.y - transform.position.y);

            if (!isKnockedBack)
            {
                rb.velocity = new Vector2(directionX * chaseSpeed, directionY * chaseSpeed);

                if (Mathf.Abs(player.transform.position.x - transform.position.x) > 0.1f)
                    Flip(directionX);

                // Stop moving when close to attack range
                if (Vector2.Distance(transform.position, player.transform.position) <= attackRange - 0.5f)
                    rb.velocity = Vector2.zero;
            }
        }
        else
            ResetToPatrol();
    }

    private void ResetToPatrol()
    {
        rb.velocity = Vector2.zero; // Stop any chase momentum
        patrolDirectionTimer = 0f; // Trigger a new patrol direction immediately
        Patrol();
    }


    protected override void ResetKnockback()
    {
        base.ResetKnockback();
        rb.velocity = Vector2.zero;
        attackTimer = 0;
        isAttacking = false;
    }

    private void Flip(float direction)
    {
        Vector3 scale = transform.localScale;
        if (direction < 0 && scale.x > 0 || direction > 0 && scale.x < 0)
        {
            scale.x = -scale.x;
            transform.localScale = scale;
        }
    }

    public override void Attack()
    {
        if (isKnockedBack || isAttacking) return; //prevent attack while knocked back
        isAttacking = true;
        attackTimer = attackCooldown;
        AudioManager.instance.PlaySFX(enemyAttack);

        //decide attack type (primary or secondary)
        if (Random.Range(0, 100) < secondaryAttackChance)
            animator.SetTrigger("SecondaryAttack");
        else
            animator.SetTrigger("PrimaryAttack");
    }

    //primary attack logic
    public void PrimaryAttack()
    {
        if (!isKnockedBack)
            player.GetComponent<Player>().TakeDamage(damage);
    }

    //secondary attack logic
    public void SecondaryAttack()
    {
        AudioManager.instance.PlaySFX(Random.Range(0, 1) == 0 ? primaryAttackClip : secondaryAttackClip);

        if (!isKnockedBack)
            player.GetComponent<Player>().TakeDamage(damage * 2);
    }

    protected override void Die()
    {
        if(isDead) return;
        base.Die();
        animator.SetTrigger("Death");
        rb.gravityScale = 1f;
        AudioManager.instance.PlaySFX(deathClip);
        Destroy(gameObject, 5f);
    }
}