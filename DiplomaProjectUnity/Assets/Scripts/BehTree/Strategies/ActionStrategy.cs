using System;
using UnityEngine;

namespace DiplomaProject.BehTree.Strategies
{
    public class ActionStrategy : IStrategy
    {
        private Action _action;

        public ActionStrategy(Action action)
        {
            _action = action;
        }
        
        public Node.Status Process()
        {
            try
            {
                _action();
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