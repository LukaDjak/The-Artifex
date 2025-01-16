using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private Slider healthBar, auraBar, staminaBar;
    [SerializeField] private TMP_Text healthText, auraText;

    [SerializeField] private int maxHealth;
    [SerializeField] private int maxAura;
    [SerializeField] private float maxStamina;

    [SerializeField] private GameObject sfxPlayerDeath;

    [HideInInspector] public int health;
    [HideInInspector] public int aura;
    [HideInInspector] public float stamina;
    [HideInInspector] public bool isDead;

    private Animator animator;

    private void Start()
    {
        health = maxHealth;
        aura = 0;

        animator = GetComponent<Animator>();

        healthBar.maxValue = maxHealth;
        auraBar.maxValue = maxAura;
        staminaBar.maxValue = maxStamina;
        UpdateBars();
        staminaBar.gameObject.SetActive(false);
    }

    public void UpdateBars()
    {
        auraText.text = aura.ToString();
        healthText.text = health.ToString();

        auraBar.DOValue(aura, 0.3f);
        healthBar.DOValue(health, 0.3f);
    }

    public void TakeDamage(int damage)
    {
        animator.SetTrigger("Hurt");
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        UpdateBars();
    }

    public void Die()
    {
        //primjer zvuka sa vježbi :)
        Instantiate(sfxPlayerDeath, transform.position, Quaternion.identity);
        //game over - show some kind of UI first
        animator.SetTrigger("Death");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}