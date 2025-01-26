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
    [SerializeField] private AudioClip primaryAttackClip;
    [SerializeField] private AudioClip secondaryAttackClip;

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
        if (currentHealth <= 0 || isKnockedBack)
            return;

        //handle flying and patrol behavior
        if (ShouldChase())
            ChasePlayer();
        else
            Patrol();

        //handle attacks
        if (ShouldAttack() && !isAttacking && attackTimer <= 0)
            Attack();

        //attack timer
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
                isAttacking = false;
        }
    }

    //move along X-axis with some variation on Y-axis for the flight time
    private void Patrol()
    {
        patrolDirectionTimer -= Time.deltaTime;

        if (patrolDirectionTimer <= 0f)
        {
            patrolRange = -patrolRange;
            patrolDirectionTimer = Random.Range(minFlightTime, maxFlightTime);
        }

        if (Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(patrolPoint.x, 0)) > 0.2f)
        {
            float direction = Mathf.Sign(patrolPoint.x - transform.position.x);
            Flip(direction);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(patrolPoint.x, transform.position.y), patrolSpeed * Time.deltaTime);
        }
        else
            patrolPoint = new Vector2(transform.position.x + patrolRange, transform.position.y);

        float yVariation = Mathf.Sin(Time.time * 2f) * 0.5f;
        transform.position = new Vector3(transform.position.x, yVariation, transform.position.z);
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
            Flip(direction);
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            //stop moving a few units before the player
            if (distanceToPlayer > attackRange - .5f)
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);
        }
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
        isAttacking = true;
        attackTimer = attackCooldown;
        AudioManager.instance.PlaySFX(enemyAttack);

        //decide attack type (primary or secondary)
        if (Random.Range(0, 100) < secondaryAttackChance)
        {
            animator.SetTrigger("SecondaryAttack");
            AudioManager.instance.PlaySFX(secondaryAttackClip);
        }
        else
        {
            animator.SetTrigger("PrimaryAttack");
            AudioManager.instance.PlaySFX(primaryAttackClip);
        }
    }

    public void PrimaryAttack() => player.GetComponent<Player>().TakeDamage(damage); //called on animation event frame

    public void SecondaryAttack() => player.GetComponent<Player>().TakeDamage(damage * 2); //called on animation event frame

    protected override void Die()
    {
        animator.SetTrigger("Death");
        AudioManager.instance.PlaySFX(spawnAndDeathClip);
        base.Die();
    }
}