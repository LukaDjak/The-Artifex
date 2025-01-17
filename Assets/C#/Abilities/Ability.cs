using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] protected string abilityName;
    [SerializeField] protected int auraCost;

    protected Player player;
    protected bool isActive;

    protected virtual void Start() => player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    public bool CanUse() => player.aura >= auraCost && !isActive;

    public void UseAbility()
    {
        if (CanUse())
        {
            player.aura -= auraCost;
            isActive = true;
            Activate();
        }
        else
        {
            Debug.Log("Not enough aura or ability already active!");
        }
    }

    protected abstract void Activate();

    public bool IsActive() => isActive;

    public string GetAbilityName() => abilityName;

    protected virtual void Deactivate()
    {
        isActive = false;
    }
}