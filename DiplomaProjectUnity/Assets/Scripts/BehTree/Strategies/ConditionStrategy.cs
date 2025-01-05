using System;

namespace DiplomaProject.BehTree.Strategies
{
    public class ConditionStrategy : IStrategy
    {
        private Func<bool> _predicate;

        public ConditionStrategy(Func<bool> predicate)
        {
            _predicate = predicate;
        }
        
        public Node.Status Process()
        {
            return _predicate() ? Node.Status.Success : Node.Status.Failure;
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}