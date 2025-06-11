public class NearbyEnemiesQuery : BehaviorTree
{
    int count;
    float distance;

    public override Result Run()
    {
        var nearby = GameManager.Instance.GetEnemiesInRange(agent.transform.position, distance);
        if (nearby.Count >= count)
        {
            return Result.SUCCESS;
        }
        return Result.FAILURE;
    }

    public NearbyEnemiesQuery(int count, float distance) : base()
    {
        this.count = count;
        this.distance = distance;
    }

    public override BehaviorTree Copy()
    {
        return new NearbyEnemiesQuery(count, distance);
    }
}
