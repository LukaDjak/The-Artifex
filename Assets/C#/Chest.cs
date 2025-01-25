using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField] private Boost[] boosts;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image IconUI;
    private Animator animator;
    private Rigidbody2D rb;
    private bool readyToOpen, isOpened = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && readyToOpen && !isOpened)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                isOpened = true;
                animator.SetTrigger("Open");
                Invoke(nameof(ShowText), .5f);
                SelectRandomBoost(player);
                Destroy(gameObject, 10f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!readyToOpen)
        {
            animator.SetTrigger("Fall");
            readyToOpen = true;
            rb.isKinematic = true;
        }
    }

    public void SelectRandomBoost(Player player)
    {
        float totalProbability = 0;
        foreach (Boost boost in boosts)
            totalProbability += boost.probability;

        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0;

        foreach (Boost boost in boosts)
        {
            cumulativeProbability += boost.probability;
            if (randomValue <= cumulativeProbability)
            {
                //show boost icon and update chest text
                boost.ApplyBoost(player);
                if (boost.boostName == "Heal")
                    text.text = $"{boost.boostName}: +{boost.boostAmount}HP";
                else if (boost.boostName == "Grenade")
                    text.text = $"{boost.boostName}: +{boost.boostAmount}";
                else
                    text.text = $"{boost.boostName}: +{boost.boostAmount * 100f}%";
                IconUI.sprite = boost.boostIcon;
                return;
            }
        }
    }

    private void ShowText() => canvas.SetActive(true);
}