using System.Collections;
using UnityEngine;

public class JumpBoost : Ability
{
    [Header("Ability Settings")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private float jumpBoostMultiplier = 1.5f;

    private float originalJumpForce;

    protected override void Activate()
    {
        Debug.Log($"Activated {abilityName}. Jump boost for {duration} seconds!");

        originalJumpForce = player.GetComponent<PlayerMovement>().jumpForce;
        player.GetComponent<PlayerMovement>().jumpForce *= jumpBoostMultiplier;

        StartCoroutine(DeactivateAbilityAfterDuration());
    }

    private IEnumerator DeactivateAbilityAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        player.GetComponent<PlayerMovement>().jumpForce = originalJumpForce;

        Debug.Log($"{abilityName} has ended.");
        Deactivate();
    }
}
