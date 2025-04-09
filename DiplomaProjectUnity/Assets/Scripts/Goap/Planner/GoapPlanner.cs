using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DiplomaProject.Goap.Planner
{
    public class GoapPlanner : IGoapPlanner
    {
        public ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null)
        {
            var orderedGoals = goals
                .Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
                .OrderByDescending(g => g == mostRecentGoal ? g.Priority - 0.01 : g.Priority)
                .ToList();

            foreach (var goal in orderedGoals)
            {
                var goalNode = new Node(null, null, goal.DesiredEffects, 0);

                if (FindPath(goalNode, agent.Actions))
                {
                    if (goalNode.IsLeafDead)
                        continue;

                    var actionStack = new Stack<AgentAction>();
                    while (goalNode.Leaves.Count > 0)
                    {
                        var cheapestLeaf = goalNode.Leaves.OrderBy(l => l.Cost).First();
                        goalNode = cheapestLeaf;
                        actionStack.Push(cheapestLeaf.Action);
                    }
                    
                    return new ActionPlan(goal, actionStack, goalNode.Cost);
                }
            }

            Debug.LogError("No plan found");
            return null;
        }

        private bool FindPath(Node parent, HashSet<AgentAction> actions)
        {
            var orderedActions = actions.OrderBy(a => a.Cost);
            
            foreach (var action in orderedActions)
            {
                var requiredEffects = parent.RequiredEffects;
                requiredEffects.RemoveWhere(b => b.Evaluate());

                if (requiredEffects.Count == 0)
                    return true;

                if (action.Effects.Any(requiredEffects.Contains))
                {
                    var newRequiredEffects = new HashSet<AgentBelief>(requiredEffects);
                    newRequiredEffects.ExceptWith(action.Effects);
                    newRequiredEffects.UnionWith(action.Preconditions);

                    var newAvailableActions = new HashSet<AgentAction>(actions);
                    newAvailableActions.Remove(action);

                    var node = new Node(parent, action, newRequiredEffects, parent.Cost + action.Cost);

                    if (FindPath(node, newAvailableActions))
                    {
                        parent.Leaves.Add(node);
                        newRequiredEffects.ExceptWith(node.Action.Preconditions);
                    }

                    if (newRequiredEffects.Count == 0)
                        return true;
                }
            }
            
            return false;
        }
    }
}