using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] private float jumpForce = 7f; // Vertical jump force
    private float attackCooldownTimer;
    Rigidbody2D rb;
    Animator animator;

    bool smallerSlime = false;
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        attackCooldownTimer -= Time.deltaTime;

        if (attackCooldownTimer <= 0f)
        {
            Jump();
            attackCooldownTimer = IsPlayerInRange() ? attackCooldown / 2 : attackCooldown;
        }
    }

    private void Jump()
    {
        float direction = IsPlayerInRange() ?
            Mathf.Sign(player.transform.position.x - transform.position.x) :
            Random.value > 0.5f ? 1f : -1f;
        rb.velocity = new Vector2(direction * jumpForce / 2, jumpForce);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") 
            && attackCooldownTimer <= 0)
            Attack();
    }

    public override void Attack() => player.GetComponent<Player>().TakeDamage(damage);
   
    protected override void Die()
    {
        Debug.Log($"{name} has died!");
        animator.SetTrigger("Death");

        // Spawn two smaller slimes
        if (!smallerSlime)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject smallSlime = Instantiate(gameObject, transform.position + new Vector3(Random.Range(-.5f, .5f), 0, 0), Quaternion.identity);
                smallSlime.transform.localScale *= 0.5f; // Shrink the smaller slimes
                smallSlime.GetComponent<Slime>().attackCooldown *= 1.5f; // Adjust cooldown for smaller slimes
                smallSlime.GetComponent<Slime>().maxHealth /= 2;
                smallSlime.GetComponent<Slime>().smallerSlime = true;
            }
        }
        base.Die();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        animator.SetTrigger("Hurt");
    }
}
