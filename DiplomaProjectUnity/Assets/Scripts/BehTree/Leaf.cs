using DiplomaProject.BehTree.Strategies;

namespace DiplomaProject.BehTree
{
    public class Leaf : Node
    {
        private IStrategy strategy;

        public Leaf(string name, IStrategy strategy) : base(name)
        {
            this.strategy = strategy;
        }

        public override Status Process() => strategy.Process();
        public override void Reset() => strategy.Reset();
    }
}