using UnityEngine;

public class Buff : BehaviorTree
{
    public override Result Run()
    {
        var target = GameManager.Instance.GetClosestOtherEnemy(agent.gameObject);
        EnemyAction act = agent.GetAction("buff");
        if (act == null) return Result.FAILURE;
        
        bool success = act.Do(target.transform);
        return (success ? Result.SUCCESS : Result.FAILURE);
    }

    public Buff() : base()
    {

    }

    public override BehaviorTree Copy()
    {
        return new Buff();
    }
}
