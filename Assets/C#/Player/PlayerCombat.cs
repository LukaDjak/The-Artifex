using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int damage = 10;  // Damage dealt by the player
    [SerializeField] private float attackRange = 1.5f;  // Attack range
    [SerializeField] private float attackCooldown = 1f;  // Cooldown between attacks

    private Animator animator;
    private float attackCooldownTimer = 0f;

    private void Start() => animator = GetComponent<Animator>();
    

    private void Update()
    {
        // Handle attack input and cooldown
        if (attackCooldownTimer <= 0f && Input.GetButtonDown("Fire1") && Time.timeScale > 0 && !LevelManager.instance.IsGameOver()) // "Fire1" is the default attack input
        {
            Attack();
            attackCooldownTimer = attackCooldown; // Reset cooldown timer
        }
        else
            attackCooldownTimer -= Time.deltaTime; // Decrease cooldown timer
    }

    private void Attack()
    {
        // Trigger attack animation
        animator.SetTrigger("Attack");

        // Detect enemies in attack range using a Physics2D overlap
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, 0.5f, 0), attackRange);

        // Loop through the enemies and apply damage if hit
        foreach (var enemyCollider in enemiesHit)
        {
            if (enemyCollider.CompareTag("Enemy"))
                enemyCollider.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    // Visualize attack range in the editor for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.5f, 0), attackRange);
    }
}
