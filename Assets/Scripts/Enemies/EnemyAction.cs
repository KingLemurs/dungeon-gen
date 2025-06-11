using UnityEngine;

public class EnemyAction 
{
    public float last_use;
    public float cooldown;
    public float range;

    public EnemyController enemy;

    public bool Ready()
    {
        return (last_use + cooldown < Time.time);
    }

    public bool CanDo(Transform target)
    {
        return Ready() && InRange(target);
    }

    public bool InRange(Transform target)
    {
        return ((target.position - enemy.transform.position).magnitude <= range);
    }

    public bool Do(Transform target)
    {
        if (!CanDo(target)) return false;
        last_use = Time.time;
        return Perform(target);
    }

    protected virtual bool Perform(Transform target)
    {
        return false;
    }

    public EnemyAction(float cooldown, float range)
    {
        this.cooldown = cooldown;
        this.range = range;
    }
}
