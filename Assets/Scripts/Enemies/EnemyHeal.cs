using UnityEngine;

public class EnemyHeal : EnemyAction
{
    int amount;

    protected override bool Perform(Transform target)
    {
        var healee = target.GetComponent<EnemyController>();
        // some targets might have a debuff
        if (healee.GetEffect("noheal") > 0) return false;
        enemy.hp.Damage(new Damage(1, Damage.Type.DARK));
        healee.hp.Heal(amount);
        return true;
    }

    public EnemyHeal(float cooldown, float range, int amount) : base(cooldown, range)
    {
        this.amount = amount;
    }
}