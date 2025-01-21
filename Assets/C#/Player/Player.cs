using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Linq;

public class Player : MonoBehaviour
{ 
    [SerializeField] private Slider healthBar, auraBar, staminaBar;
    [SerializeField] private TMP_Text healthText, auraText;
    [SerializeField] private int maxHealth, maxAura;
    [SerializeField] private float maxStamina;
    [SerializeField] private GameObject sfxPlayerDeath;

    [HideInInspector] public int health, aura;
    [HideInInspector] public float stamina;
    [HideInInspector] public bool infiniteStamina;

    private Animator animator;
    private readonly float staminaRechargeRate = 75f, staminaRechargeDelay = 2f;
    private float rechargeTimer;

    private void Start()
    {
        GameManager.gameData.total_games_played++;

        health = maxHealth;
        aura = 0;
        stamina = maxStamina;
        animator = GetComponent<Animator>();

        healthBar.maxValue = maxHealth;
        auraBar.maxValue = maxAura;
        staminaBar.maxValue = maxStamina;
        staminaBar.gameObject.SetActive(false);

        UpdateBars();
    }

    private void Update()
    {
        HandleStamina();
        staminaBar.value = stamina;
        staminaBar.gameObject.SetActive(stamina < maxStamina);
    }

    public void UpdateBars()
    {
        healthText.text = health.ToString();
        auraText.text = aura.ToString();
        healthBar.DOValue(health, 0.3f);
        auraBar.DOValue(aura, 0.3f);
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
        UpdateBars();
    }

    public void Die()
    {
        Instantiate(sfxPlayerDeath, transform.position, Quaternion.identity);
        animator.SetTrigger("Death");
        LevelManager.instance.GameOver();
    }
}