using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Attributes")]
    [SerializeField] private string enemyName;
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] private int minAura;
    [SerializeField] private int maxAura;

    [Header("Attack Settings")]
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float chaseRange = 10f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackCooldown = 1.5f;

    [HideInInspector] public int currentHealth;

    protected GameObject player;
    protected bool isAttacking;
    protected float attackTimer;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected virtual void Update()
    {
        //attack cooldown
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
                isAttacking = false;
        }
    }

    public abstract void Attack();

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;

        //hurt animation, update enemy health bar

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        //some kind of death effect or animation

        player.GetComponent<Player>().aura += Random.Range(minAura, maxAura);
        Destroy(gameObject);
    }

    protected bool IsPlayerInRange() =>
        Vector2.Distance(transform.position, player.transform.position) <= chaseRange;

    private void OnDrawGizmosSelected()
    {
        // Visualize attack range in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}