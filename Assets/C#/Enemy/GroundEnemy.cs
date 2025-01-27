using UnityEngine;
using UnityEngine.Rendering;

public class GroundEnemy : Enemy
{
    [Header("Movement Settings")]
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private float wanderDuration = 3f;
    [SerializeField] private float speed = 2f;
    [Range(1f, 2f)]
    [SerializeField] private float chaseSpeedMultiplier = 1f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float obstacleDetectionDistance = 1f; //distance to detect obstacles
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip[] deathClips;

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
            rb.velocity = new Vector2(0, rb.velocity.y);
    }


    private void ChasePlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;

        if (IsObstacleInPath())
            JumpOverObstacle();
        else
        {
            rb.velocity = new Vector2(direction.x * speed * chaseSpeedMultiplier, rb.velocity.y);
            Flip(direction.x);
        }
    }

    private bool IsObstacleInPath()
    {
        //check for obstacles in front of the enemy using a raycast
        Vector2 origin = transform.position - new Vector3(0, .3f, 0);
        Vector2 direction = new(transform.localScale.x, 0); //forward direction based on facing
        Debug.DrawRay(origin, direction, Color.red);
        return Physics2D.Raycast(origin, direction, obstacleDetectionDistance, groundLayer);
    }

    private void JumpOverObstacle() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    private void ResetToIdle()
    {
        if (isAttacking || ShouldAttack() || ShouldChase()) return; // Prevent resetting to idle if attacking, in attack range, or chasing
        isChasing = false;
        stateTimer = idleDuration;
        isWandering = false;
        wanderDirection = Vector2.zero;
        animator.SetBool("IsMoving", false);
    }

    public override void Attack()  // call this on Animation
    {
        if (isAttacking) return;
        isAttacking = true;
        attackTimer = attackCooldown;

        player.GetComponent<Player>().TakeDamage(damage);
        if (attackClip != null)
            AudioManager.instance.PlaySFX(attackClip); //specified clip - specific for each enemy
        AudioManager.instance.PlaySFX(enemyAttack); //base clip - every enemy
        animator.SetTrigger("Attack");
    }

    protected override void Die()
    {
        base.Die();
        AudioManager.instance.PlaySFX(deathClips[Random.Range(0, deathClips.Length)]);
        if (HasParameter(animator, "Death"))
        {
            animator.SetTrigger("Death");
            rb.velocity = Vector2.zero;
            Destroy(gameObject, 5f);
        }
        else
            Destroy(gameObject);
    }

    // Helper method to check if a trigger exists in the animator
    private bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName && param.type == AnimatorControllerParameterType.Trigger)
                return true;
        }
        return false;
    }

    private void Flip(float direction)
    {
        if (Mathf.Approximately(direction, 0)) return;
        transform.localScale = new Vector3(Mathf.Sign(direction) * transform.localScale.y, transform.localScale.y, transform.localScale.z);
    }
}