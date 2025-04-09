using System.Collections.Generic;

namespace DiplomaProject.Goap
{
    public class ActionPlan
    {
        public AgentGoal ActionGoal { get; }
        public Stack<AgentAction> Actions { get; }
        public float TotalCost { get; set; }
        
        public ActionPlan(AgentGoal actionGoal, Stack<AgentAction> actions, float totalCost)
        {
            ActionGoal = actionGoal;
            Actions = actions;
            TotalCost = totalCost;
        }
    }
}