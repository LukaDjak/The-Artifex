using System.Collections;
using UnityEngine;

public class InfinityStamina : Ability
{
    [Header("Ability Settings")]
    [SerializeField] private float duration = 5f;

    private Coroutine infinityStaminaCoroutine;

    protected override void Activate()
    {
        if (infinityStaminaCoroutine != null)
            StopCoroutine(infinityStaminaCoroutine);

        infinityStaminaCoroutine = StartCoroutine(GrantInfinityStamina());
    }

    private IEnumerator GrantInfinityStamina()
    {
        Debug.Log($"Activated {abilityName}. Unlimited stamina for {duration} seconds!");

        player.infiniteStamina = true;
        yield return new WaitForSeconds(duration);
        player.infiniteStamina = false;

        Debug.Log($"{abilityName} has ended.");
        Deactivate();
    }
}