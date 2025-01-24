using UnityEngine;
using TMPro;
using UnityEditor.EditorTools;

[System.Serializable]
public class Boost
{
    public string boostName;
    public Sprite boostIcon;
    public float probability;

    [Tooltip("In %, except grenades (amount)")]
    public float boostAmount;

    public void ApplyBoost(Player player)
    {
        switch(boostName)
        {
            case "MaxHealth":
                player.chestMultipliers.maxHealthMultiplier += boostAmount * player.chestMultipliers.maxHealthMultiplier;
                break;
            case "AuraBoost":
                player.chestMultipliers.auraMultiplier += boostAmount * player.chestMultipliers.auraMultiplier;
                break;
            case "SpeedBoost":
                player.chestMultipliers.speedMultiplier += boostAmount * player.chestMultipliers.speedMultiplier;
                break;
            case "DamageBoost":
                player.chestMultipliers.damageMultiplier++;
                break;
            case "StaminaBoost":
                player.chestMultipliers.staminaDecreaseMultiplier += boostAmount * player.chestMultipliers.staminaDecreaseMultiplier;
                break;
            case "Heal":
                player.health += (int)boostAmount;
                break;
            case "Grenade":
                player.chestMultipliers.grenades++;
                break;
            case "ReviveChance":
                player.chestMultipliers.reviveChanceMultiplier += boostAmount;
                break;
            default:
                break; //call here again if needed - PLAYER MUST GET SOMETHING FROM EVERY CHEST
        }
    }
}

public class Chest : MonoBehaviour
{
    [SerializeField] private Boost[] boosts;
    [SerializeField] private TMP_Text text;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                SelectRandomBoost(player);
                Destroy(gameObject); 
                //trigger chest opening animation and destroy it after 10-15 seconds + particles ofc
            }
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
                Debug.Log($"Applied Boost: {boost.boostName}");
                return;
            }
        }
    }
}