using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Attributes")]
    [SerializeField] private string enemyName;
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] private int minAura;
    [SerializeField] private int maxAura;
    public event Action OnDeath;
    [SerializeField] protected AudioClip enemyAttack;

    [Header("Hurt Flash")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration;

    [Header("Attack Settings")]
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float chaseRange = 10f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackCooldown = 1.5f;

    [HideInInspector] public int currentHealth;

    protected GameObject player;
    protected bool isAttacking;
    protected float attackTimer;

    private SpriteRenderer sr;
    private Material originalMaterial;
    private Coroutine flashCoroutine;
    protected bool isKnockedBack = false;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    protected virtual void Update()
    {
        if (isKnockedBack) return;
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
        if(currentHealth > 0) 
            Flash();
        else
            Die();
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (isKnockedBack) return; //prevent multiple knockbacks at once
        isKnockedBack = true;

        rb.velocity = Vector2.zero;
        rb.AddForce(force * rb.mass * direction, ForceMode2D.Impulse);

        Invoke(nameof(ResetKnockback), .2f);
    }

    protected virtual void ResetKnockback() => isKnockedBack = false;

    protected virtual void Die()
    {
        //some kind of death effect or animation
        OnDeath?.Invoke();
        int auraToGive = (int)(UnityEngine.Random.Range(minAura, maxAura) * player.GetComponent<Player>().chestMultipliers.auraMultiplier);
        player.GetComponent<Player>().aura += auraToGive;
        player.GetComponent<Player>().UpdateBars();
        GameManager.gameData.total_aura += auraToGive;
        GameManager.gameData.total_kills++;

        TryDropArtifact(transform.position);

        Destroy(gameObject);
    }

    private void TryDropArtifact(Vector2 spawnPosition)
    {
        if (UnityEngine.Random.Range(0, 10000) != 0) return;

        List<Artifact> uncollected = GameManager.Instance.artifacts.FindAll(artifact =>
            !GameManager.Instance.collectedArtifacts.Contains(artifact.name));

        if (uncollected.Count > 0)
        {
            Artifact randomArtifact = uncollected[UnityEngine.Random.Range(0, uncollected.Count)];
            Instantiate(randomArtifact.prefab, spawnPosition, Quaternion.identity)
                .GetComponent<ArtifactPickup>()
                .Setup(randomArtifact.name);
        }
    }

    protected bool ShouldChase() =>
        Vector2.Distance(transform.position, player.transform.position) <= chaseRange && !LevelManager.instance.IsGameOver();    
    
    protected bool ShouldAttack() =>
        Vector2.Distance(transform.position, player.transform.position) <= attackRange && !LevelManager.instance.IsGameOver();

    protected void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        sr.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        sr.material = originalMaterial;
        flashCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize attack range in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}

/* juicing up enemies
- hurt flash - done
- blood particles
- little knockback - done
- audio effect - done
*/