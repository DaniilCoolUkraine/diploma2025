using System.Collections.Generic;

namespace DiplomaProject.Goap.Planner
{
    public class Node
    {
        public Node Parent { get; }
        public AgentAction Action;
        public HashSet<AgentBelief> RequiredEffects { get; }
        public List<Node> Leaves { get; }
        public float Cost { get; }

        public Node(Node parent, AgentAction action, HashSet<AgentBelief> effects, float cost)
        {
            Parent = parent;
            Action = action;
            RequiredEffects = new HashSet<AgentBelief>(effects);
            Leaves = new List<Node>();
            Cost = cost;
        }
        
        public bool IsLeafDead => Leaves.Count == 0 && Action == null;
    }
}