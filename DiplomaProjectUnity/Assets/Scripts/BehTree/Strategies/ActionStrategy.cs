using System;
using UnityEngine;

namespace DiplomaProject.BehTree.Strategies
{
    public class ActionStrategy : IStrategy
    {
        private Action _predicate;

        public ActionStrategy(Action predicate)
        {
            _predicate = predicate;
        }
        
        public Node.Status Process()
        {
            try
            {
                _predicate();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return Node.Status.Failure;
            }
            return Node.Status.Success;
        }
    }
}