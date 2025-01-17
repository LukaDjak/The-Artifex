using UnityEngine;

public class Dash : Ability
{
    [Header("Ability Settings")]
    [SerializeField] private float duration = 10f;  // Total ability duration

    private PlayerMovement playerMovement;

    protected override void Start()
    {
        base.Start();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    protected override void Activate()
    {
        playerMovement.dashActive = true;
        Debug.Log($"Activated {abilityName}. Dash for {duration} seconds!");
        Invoke(nameof(Deactivate), duration);
    }

    protected override void Deactivate()
    {
        playerMovement.dashActive = false;
        Debug.Log($"{abilityName} has ended.");
        base.Deactivate();
    }
}