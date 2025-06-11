public class AbilityReadyQuery : BehaviorTree
{
    string ability;

    public override Result Run()
    {
        if (agent.GetAction(ability).Ready())
        {
            return Result.SUCCESS;
        }
        return Result.FAILURE;
    }

    public AbilityReadyQuery(string ability) : base()
    {
        this.ability = ability;
    }

    public override BehaviorTree Copy()
    {
        return new AbilityReadyQuery(ability);
    }
}
