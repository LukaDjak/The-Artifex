using UnityEngine;

public class Eye : Enemy
{
    [Header("Flying Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float patrolRange = 5f;
    [SerializeField] private float minFlightTime = 2f;
    [SerializeField] private float maxFlightTime = 5f;

    [Header("Attack Settings")]
    [SerializeField] private int secondaryAttackChance = 20;

    [Space(15f)]
    [SerializeField] private AudioClip spawnAndDeathClip;

    private float patrolDirectionTimer = 0f;

    private Vector2 patrolPoint;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        patrolPoint = new Vector2(transform.position.x + patrolRange, transform.position.y);
        AudioManager.instance.PlaySFX(spawnAndDeathClip);
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        if (currentHealth <= 0)
            return;

        //handle flying and patrol behavior
        if (ShouldChase())
            ChasePlayer();
        else
            Patrol();

        //handle attacks
        if (ShouldAttack() && !isAttacking && attackTimer <= 0)
            Attack();

        // Update the attack timer
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
                isAttacking = false;
        }
    }

    private void Patrol()
    {
        // Update the timer
        patrolDirectionTimer -= Time.deltaTime;

        // Change patrol direction every 2-3 seconds
        if (patrolDirectionTimer <= 0f)
        {
            // Reverse patrol direction
            patrolRange = -patrolRange;

            // Reset the timer with a random interval between 2 and 5 seconds
            patrolDirectionTimer = Random.Range(minFlightTime, maxFlightTime);
        }

        // Move along the X axis only
        if (Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(patrolPoint.x, 0)) > 0.2f)
        {
            // Determine the direction of movement and flip accordingly
            float direction = Mathf.Sign(patrolPoint.x - transform.position.x);
            Flip(direction);

            // Move the enemy on the X axis only
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(patrolPoint.x, transform.position.y), patrolSpeed * Time.deltaTime);
        }
        else
        {
            // When the patrol point is reached, set a new patrol point in the opposite direction
            patrolPoint = new Vector2(transform.position.x + patrolRange, transform.position.y);
        }

        // Simulate natural flying height variation (Y-axis)
        float yVariation = Mathf.Sin(Time.time * 2f) * 0.5f;
        transform.position = new Vector3(transform.position.x, yVariation, transform.position.z);
    }


    private void ChasePlayer()
    {
        if (player != null)
        {
            // Determine the direction of movement and flip accordingly
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
            Flip(direction);

            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);
        }
    }

    private void Flip(float direction)
    {
        // Flip the sprite by changing the x scale to negative or positive based on movement direction
        Vector3 scale = transform.localScale;
        if (direction < 0 && scale.x > 0 || direction > 0 && scale.x < 0)
        {
            scale.x = -scale.x;
            transform.localScale = scale;
        }
    }

    public override void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;
        AudioManager.instance.PlaySFX(enemyAttack);


        //decide attack type (primary or secondary)
        if (Random.Range(0, 100) < secondaryAttackChance)
            animator.SetTrigger("SecondaryAttack");
        else
            animator.SetTrigger("PrimaryAttack");
    }

    public void PrimaryAttack()
    {
        //perform primary melee attack
        player.GetComponent<Player>().TakeDamage(damage);
        
    }

    public void SecondaryAttack()
    {
        //perform secondary melee attack with a different effect or animation
        player.GetComponent<Player>().TakeDamage(damage * 2); // Example of stronger damage
    }

    protected override void Die()
    {
        animator.SetTrigger("Death");
        AudioManager.instance.PlaySFX(spawnAndDeathClip);
        base.Die();
    }
}