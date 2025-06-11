using UnityEngine;

public class EnemyAttack : EnemyAction
{
    int damage;
    public int AttackDamage {  get { return damage;  } }
    
    float strength_factor;
    public float StrengthFactor { get { return strength_factor; } }

    protected override bool Perform(Transform target)
    {
        int amount = damage;
        amount += Mathf.RoundToInt(enemy.GetEffect("strength")*strength_factor);
        GameManager.Instance.player.GetComponent<PlayerController>().hp.Damage(new Damage(amount, Damage.Type.PHYSICAL));
        return true;     
    }

    public EnemyAttack(float cooldown, float range, int damage, float strength_factor): base(cooldown, range)
    {
        this.damage = damage;
        this.strength_factor = strength_factor;
    }
}
