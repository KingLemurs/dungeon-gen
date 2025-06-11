using UnityEngine;
using System.Collections.Generic;

public class BehaviorTree 
{
    public enum Result { SUCCESS, FAILURE, IN_PROGRESS };

    public EnemyController agent;

    public virtual Result Run()
    {
        return Result.SUCCESS;
    }

    public BehaviorTree()
    {

    }

    public void SetAgent(EnemyController agent)
    {
        this.agent = agent;
    }

    public virtual IEnumerable<BehaviorTree> AllNodes()
    {
        yield return this;
    }

    public virtual BehaviorTree Copy()
    {
        return null;
    }
}
