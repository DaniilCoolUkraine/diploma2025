using UnityEngine;

namespace DiplomaProject.BehTree
{
    public class LoopSelector : Node
    {
        public LoopSelector(string name) : base(name)
        {
        }

        public override Status Process()
        {
            if (currentChild < Children.Count)
            {
                switch (Children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Running;
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