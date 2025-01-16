using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] private float jumpForce = 7f; //vertical jump force

    private float jumpCooldownTimer;
    private float attackCooldownTimer;

    private Rigidbody2D rb;
    private Animator animator;

    private bool smallerSlime = false;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        jumpCooldownTimer = attackCooldown; //initialize cooldown timers
        attackCooldownTimer = 0f;
    }

    protected override void Update()
    {
        //update timers
        attackCooldownTimer -= Time.deltaTime;
        jumpCooldownTimer -= Time.deltaTime;

        //handle jumping
        if (jumpCooldownTimer <= 0f)
        {
            Jump();
            jumpCooldownTimer = ShouldChase() ? attackCooldown / 2 : attackCooldown; //reset jump cooldown
        }
    }

    private void Jump()
    {
        //determine jump direction (towards player if in range, otherwise random)
        float direction = ShouldChase()
            ? Mathf.Sign(player.transform.position.x - transform.position.x)
            : Random.value > 0.5f ? 1f : -1f;

        rb.velocity = new Vector2(direction * jumpForce / 2, jumpForce);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && attackCooldownTimer <= 0f)
        {
            Attack();
            attackCooldownTimer = attackCooldown; //reset attack cooldown
        }
    }

    public override void Attack() => player.GetComponent<Player>().TakeDamage(damage);
    
    protected override void Die()
    {
        animator.SetTrigger("Death");

        //spawn smaller slimes
        if (!smallerSlime)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject smallSlime = Instantiate(gameObject,
                    transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0),
                    Quaternion.identity);

                var slimeComponent = smallSlime.GetComponent<Slime>();
                smallSlime.transform.localScale *= 0.75f;
                slimeComponent.attackCooldown *= 1.5f;
                slimeComponent.maxHealth /= 2;
                slimeComponent.smallerSlime = true;
            }
        }
        base.Die();
    }
}