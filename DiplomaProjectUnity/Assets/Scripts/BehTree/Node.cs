using System.Collections.Generic;

namespace DiplomaProject.BehTree
{
    public class Node
    {
        public enum Status
        {
            Success,
            Failure,
            Running
        }

        public readonly string Name;

        public readonly List<Node> Children = new();
        protected int currentChild;

        #region Constructors

        public Node()
        {
            Name = "Node";
        }
        
        public Node(string name)
        {
            Name = name;
        }

        #endregion

        public Node AddChild(Node child)
        {
            Children.Add(child);
            return this;
        }

        public virtual Status Process()
        {
            return Children[currentChild].Process();
        }

        public virtual void Reset()
        {
            currentChild = 0;
            Children.ForEach(c => c.Reset());
        }
    }
}