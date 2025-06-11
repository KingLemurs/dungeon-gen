using UnityEngine;
using System;

public class EventBus 
{
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    public event Action<Vector3, Damage, Hittable> OnDamage;
    
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
    }

    public event Action<Vector3, int, Hittable> OnHeal;

    public void DoHeal(Vector3 where, int amount, Hittable target)
    {
        OnHeal?.Invoke(where, amount, target);
    }

    public event Action<EnemyController> OnEnemyDeath;

    public void DoEnemyDeath(EnemyController which)
    {
        OnEnemyDeath?.Invoke(which);
    }
}
