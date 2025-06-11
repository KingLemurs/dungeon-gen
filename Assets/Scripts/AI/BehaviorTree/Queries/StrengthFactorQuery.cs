public class StrengthFactorQuery : BehaviorTree
{
    float min_strength_factor;

    public override Result Run()
    {
        var target = GameManager.Instance.GetClosestOtherEnemy(agent.gameObject);
        if (((EnemyAttack)target.GetComponent<EnemyController>().GetAction("attack")).StrengthFactor >= min_strength_factor)
        {
            return Result.SUCCESS;
        }
        return Result.FAILURE;
    }

    public StrengthFactorQuery(float min_strength_factor) : base()
    {
        this.min_strength_factor = min_strength_factor;
    }

    public override BehaviorTree Copy()
    {
        return new StrengthFactorQuery(min_strength_factor);
    }
}
