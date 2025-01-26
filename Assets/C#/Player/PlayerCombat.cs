using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip enemyHurt;

    private Animator animator;
    private float attackCooldownTimer = 0f;

    private void Start() => animator = GetComponent<Animator>();
    
    private void Update()
    {
        //handle attack input and cooldown
        if (attackCooldownTimer <= 0f && Input.GetButtonDown("Fire1") && Time.timeScale > 0 && !LevelManager.instance.IsGameOver() && !LevelManager.instance.isUIActive) // "Fire1" is the default attack input
        {
            Attack();
            attackCooldownTimer = attackCooldown;
        }
        else
            attackCooldownTimer -= Time.deltaTime;
    }

    private void Attack()
    {
        //trigger attack animation
        animator.SetTrigger("Attack");
        AudioManager.instance.PlaySFX(attackSound);

        //detect enemies in attack range using a Physics2D overlap
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, 0.5f, 0), attackRange, LayerMask.GetMask("Enemy"));

        bool playedHurtSound = false;

        //loop through the enemies and apply damage if hit
        for (int i = 0; i < enemiesHit.Length; i++)
        {
            if (enemiesHit[i].TryGetComponent(out Enemy e))
            {
                if (e.isDead)
                    continue;  //skip dead enemies

                e.TakeDamage(damage); 
                ApplyKnockback(enemiesHit[i]);

                if (!playedHurtSound)
                {
                    AudioManager.instance.PlaySFX(enemyHurt);
                    playedHurtSound = true;
                }
            }
        }
    }

    private void ApplyKnockback(Collider2D enemyCollider)
    {
        if (enemyCollider.TryGetComponent<Enemy>(out var enemy))
        {
            //calculate knockback direction
            Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
            knockbackDirection.y = Mathf.Abs(knockbackDirection.y);

            float knockbackForce = 10f;
            enemy.ApplyKnockback(knockbackDirection, knockbackForce);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.5f, 0), attackRange);
    }
}