using UnityEngine;
using System.Collections;

public class EnemyBuff : EnemyAction
{
    int amount;
    int duration;

    protected override bool Perform(Transform target)
    {
        var healee = target.GetComponent<EnemyController>();

        healee.AddEffect("strength", amount);
        if (duration > 0)
        {
            healee.StartCoroutine(Expire(healee));
        }
        
        return true;
    }

    public IEnumerator Expire(EnemyController healee)
    {
        yield return new WaitForSeconds(duration);
        healee.AddEffect("strength", -amount);
    }

    public EnemyBuff(float cooldown, float range, int amount, int duration) : base(cooldown, range)
    {
        this.amount = amount;
        this.duration = duration;
    }

    public EnemyBuff(float cooldown, float range, int amount) : base(cooldown, range)
    {
        this.amount = amount;
        this.duration = -1;
    }
}