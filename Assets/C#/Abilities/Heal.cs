using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Ability
{
    [Header("Heal Over Time Settings")]
    [SerializeField] private int healPerSecond = 5;
    [SerializeField] private float duration = 10f;

    protected override void Activate()
    {
        Debug.Log($"Activated {abilityName}. Healing {healPerSecond} health per second for {duration} seconds!");
        InvokeRepeating(nameof(ApplyHealing), 0f, 1f); //apply healing every second
        Invoke(nameof(Deactivate), duration);         //deactivate ability after duration
    }

    private void ApplyHealing()
    {
        if (!isActive) return;

        //heal the player
        player.Heal(healPerSecond);
    }

    protected override void Deactivate()
    {
        //stop applying healing
        CancelInvoke(nameof(ApplyHealing));
        Debug.Log($"{abilityName} has ended.");
        base.Deactivate();
    }
}
