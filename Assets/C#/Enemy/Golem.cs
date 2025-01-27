using UnityEngine;
using UnityEngine.UIElements;

public class Golem : GroundEnemy
{
    [SerializeField] private GameObject noArmorAttackFX;
    [SerializeField] private GameObject armorAttackFX;
    private bool hasArmor = true; //this will track if the enemy still has armor
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        animator.SetBool("HasArmor", true);
    }

    protected override void Die()
    {
        if (hasArmor)
            BreakArmor();
        else
            base.Die();
    }

    private void BreakArmor()
    {
        hasArmor = false;
        animator.SetTrigger("Break");
        animator.SetBool("HasArmor", false);
        maxHealth *= 2;
        damage *= 2;
        currentHealth = maxHealth;
    }

    public void AttackFX() //call on animation
    {
        GameObject fx = Instantiate(hasArmor ? armorAttackFX : noArmorAttackFX, transform.position + new Vector3(0, -0.296f, 0), Quaternion.identity);
        Destroy(fx, .5f);
    }
}