namespace DiplomaProject.BehTree
{
    public class Sequence : Node
    {
        public Sequence(string name) : base(name){}

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
                        return currentChild >= Children.Count ? Status.Success : Status.Running;
                }
            }
            
            Reset();
            return Status.Success;
        }
    }
}