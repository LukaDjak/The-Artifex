using UnityEngine;

[System.Serializable]
public class Boost
{
    public string boostName;
    public Sprite boostIcon;
    public float probability;

    [Tooltip("In %, except grenades and heal (amount)")]
    public float boostAmount;

    public void ApplyBoost(Player player)
    {
        switch(boostName)
        {
            case "Max Health":
                player.chestMultipliers.maxHealthMultiplier += boostAmount;
                player.IncreaseMaxHealth();
                player.health += (int)(boostAmount * 100f); //heal player for the boosted amount
                break;
            case "Aura Boost":
                player.chestMultipliers.auraMultiplier += boostAmount * player.chestMultipliers.auraMultiplier;
                break;
            case "Speed Boost":
                player.chestMultipliers.speedMultiplier += boostAmount * player.chestMultipliers.speedMultiplier;
                break;
            case "Damage Boost":
                player.chestMultipliers.damageMultiplier++;
                break;
            case "Stamina Boost":
                player.chestMultipliers.maxStaminaMultiplier += boostAmount;
                player.IncreaseMaxStamina();
                player.stamina += (int)(boostAmount * 100f);
                break;
            case "Heal":
                player.GiveHealth((int)boostAmount);
                break;
            case "Grenade":
                player.chestMultipliers.grenades++;
                break;
            case "Revive Chance":
                player.chestMultipliers.reviveChanceMultiplier += boostAmount;
                break;
            default:
                break; //call here again if needed - PLAYER MUST GET SOMETHING FROM EVERY CHEST
        }
        player.UpdateMaxValues();
        player.UpdateBars();
    }
}