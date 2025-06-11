using UnityEngine;

public class GoTo : BehaviorTree
{
    Transform target;
    float arrived_distance;

    public override Result Run()
    {
        Vector3 direction = target.position - agent.transform.position;
        if (direction.magnitude < arrived_distance)
        {
            agent.GetComponent<Unit>().movement = new Vector2(0, 0);
            return Result.SUCCESS;
        }
        else
        {
            agent.GetComponent<Unit>().movement = direction.normalized;
            return Result.IN_PROGRESS;
        }
    }

    public GoTo(Transform target, float arrived_distance) : base()
    {
        this.target = target;
        this.arrived_distance = arrived_distance;
    }

    public override BehaviorTree Copy()
    {
        return new GoTo(target, arrived_distance);
    }
}

