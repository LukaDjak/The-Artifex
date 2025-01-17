using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private Slider healthBar, auraBar, staminaBar;
    [SerializeField] private TMP_Text healthText, auraText;

    [SerializeField] private int maxHealth, maxAura;
    [SerializeField] private float maxStamina;

    [SerializeField] private GameObject sfxPlayerDeath;

    [HideInInspector] public int health, aura;
    [HideInInspector] public float stamina;

    [HideInInspector] public bool isDead, isSprinting, infiniteStamina;

    private Animator animator;
    private float rechargeTimer;

    private readonly float staminaDrainRate = 50f, staminaRechargeRate = 75f, staminaRechargeDelay = 2f;


    private void Start()
    {
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
        staminaBar.gameObject.SetActive(stamina < maxStamina || isSprinting);
    }

    public void UpdateBars()
    {
        healthText.text = health.ToString();
        auraText.text = aura.ToString();
        healthBar.DOValue(health, 0.3f);
        auraBar.DOValue(aura, 0.3f);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartSprinting(float xInput)
    {
        if (stamina > 0 && Mathf.Abs(xInput) > 0)
        {
            isSprinting = true;
            rechargeTimer = 0f;
        }
    }

    public void StopSprinting() => isSprinting = false;
    
    private void HandleStamina()
    {
        if (infiniteStamina)
            stamina = maxStamina;
        else if (isSprinting)
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            rechargeTimer = 0f;
        }
        else if ((rechargeTimer += Time.deltaTime) >= staminaRechargeDelay)
            stamina += staminaRechargeRate * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }
}