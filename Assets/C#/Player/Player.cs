using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour
{ 
    [SerializeField] private Slider healthBar, auraBar, staminaBar;
    [SerializeField] private TMP_Text healthText, auraText;
    [SerializeField] private int maxHealth, maxAura;
    [SerializeField] private float maxStamina;
    [SerializeField] private AudioClip playerHurt;

    [HideInInspector] public Multipliers chestMultipliers = new();
    [HideInInspector] public int health, aura;
    [HideInInspector] public float stamina;
    [HideInInspector] public bool infiniteStamina;

    private Animator animator;
    private readonly float staminaRechargeRate = 75f, staminaRechargeDelay = 2f;
    private float rechargeTimer;

    private void Start()
    {
        GameManager.gameData.total_games_played++;
        GameManager.gameData.chance_to_respawn = (int)CalculateRespawnChance();
        health = maxHealth;
        aura = 0;
        stamina = maxStamina;
        animator = GetComponent<Animator>();

        UpdateMaxValues();
        UpdateBars();
        staminaBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleStamina();
        staminaBar.value = stamina;
        staminaBar.gameObject.SetActive(stamina < maxStamina);
    }
    private float CalculateRespawnChance() => Mathf.Clamp((GameManager.gameData.total_aura / 10) * 25f, 0, 25f);

    public void UpdateBars()
    {
        healthText.text = health.ToString();
        auraText.text = aura.ToString();
        healthBar.DOValue(health, 0.3f);
        auraBar.DOValue(aura, 0.3f);
    }

    public void UpdateMaxValues()
    {
        healthBar.maxValue = maxHealth;
        auraBar.maxValue = maxAura;
        staminaBar.maxValue = maxStamina;
    }

    public void ReduceStamina(float amount)
    {
        if (!infiniteStamina)
        {
            stamina -= amount * Time.deltaTime;
            rechargeTimer = 0f;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }

    private void HandleStamina()
    {
        if (!infiniteStamina && rechargeTimer >= staminaRechargeDelay)
            stamina += staminaRechargeRate * Time.deltaTime;

        rechargeTimer += Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        else
            animator.SetTrigger("Hurt");
        AudioManager.instance.PlaySFX(playerHurt);
        UpdateBars();
    }

    public void GiveHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
    }

    public void Die()
    {
        AudioManager.instance.PlaySFX(playerHurt);
        animator.SetTrigger("Death");
        if (Random.Range(0, 100) <= GameManager.gameData.chance_to_respawn + chestMultipliers.reviveChanceMultiplier * 100)
            Invoke(nameof(Recover), 2f);
        else
            LevelManager.instance.GameOver();
    }

    public void Recover()
    {
        animator.SetTrigger("Recover");
        health = maxHealth;
        aura = 0;
        UpdateBars();
        //instantiate some kind of aura particle that kills every single enemy in its range
    }

    public void IncreaseMaxHealth() => maxHealth = (int)(maxHealth * chestMultipliers.maxHealthMultiplier);
    public void IncreaseMaxStamina() => maxStamina = (int)(maxStamina * chestMultipliers.maxStaminaMultiplier);
}

public class Multipliers
{
    public float maxHealthMultiplier = 1f;
    public float auraMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float maxStaminaMultiplier = 1f;
    public float healAmount = 50f;
    public int grenades = 0; //useless for now
    public float reviveChanceMultiplier = 0f;
}