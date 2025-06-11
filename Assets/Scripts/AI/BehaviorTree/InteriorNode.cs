using System.Collections.Generic;

public class InteriorNode : BehaviorTree
{
    protected List<BehaviorTree> children;
    protected int current_child;

    public InteriorNode(IEnumerable<BehaviorTree> children) : base()
    {
        this.children = new List<BehaviorTree>();
        this.children.AddRange(children);
        current_child = 0;
    }

    public List<BehaviorTree> CopyChildren()
    {
        List<BehaviorTree> new_children = new List<BehaviorTree>();
        foreach (var c in children)
        {
            new_children.Add(c.Copy());
        }
        return new_children;
    }

    public override IEnumerable<BehaviorTree> AllNodes()
    {
        yield return this;
        foreach (var c in children)
        {
            foreach (var n in c.AllNodes())
                yield return n;
        }
    }
}
