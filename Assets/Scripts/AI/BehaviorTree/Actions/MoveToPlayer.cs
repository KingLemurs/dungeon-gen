using UnityEngine;

public class MoveToPlayer : BehaviorTree
{
    float arrived_distance;

    public override Result Run()
    {
        Vector3 direction = GameManager.Instance.player.transform.position - agent.transform.position;
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

    public MoveToPlayer(float arrived_distance) : base()
    {
        this.arrived_distance = arrived_distance;
    }

    public override BehaviorTree Copy()
    {
        return new MoveToPlayer(arrived_distance);
    }
}
