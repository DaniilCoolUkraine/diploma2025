namespace DiplomaProject.BehTree
{
    public class BehaviourTree : Node
    {
        public BehaviourTree(string name) : base(name) { }

        public override Status Process()
        {
            while (currentChild < Children.Count)
            {
                var status = Children[currentChild].Process();
                if (status != Status.Success)
                {
                    return status;
                }

                currentChild++;
            }

            return Status.Success;
        }
    }
}