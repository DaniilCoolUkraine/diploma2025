namespace DiplomaProject.BehTree
{
    public class LoopSequence : Node
    {
        public LoopSequence(string name) : base(name){}

        public override Status Process()
        {
            if (currentChild < Children.Count)
            {
                switch (Children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    default:
                        currentChild++;
                        return Status.Running;
                }
            }
            
            Reset();
            return Status.Running;
        }
    }
}